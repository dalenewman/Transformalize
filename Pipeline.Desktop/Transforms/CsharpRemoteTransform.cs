#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using Pipeline.Contracts;

namespace Pipeline.Desktop.Transforms {

    public class CsharpRemoteTransform : CSharpBaseTransform {

        private AppDomain _domain;
        private Sponsor _sponsor;

        public CsharpRemoteTransform(IContext context) : base(context) {

        }

        public override IEnumerable<IRow> Transform(IEnumerable<IRow> rows) {

            var input = MultipleInput();
            var code = WrapCode(input, Context.Transform.Script, Context.Entity.IsMaster);

            Context.Debug((() => code));

            Context.Info("Creating new app domain and sponsor.");
            _domain = AppDomain.CreateDomain(Context.Key);
            _sponsor = new Sponsor();  // manages lifetime of the object in otherDomain

            var dll = Path.Combine(AssemblyDirectory, "Pipeline.Desktop.dll");
            using (var compilerRunner = (CompilerRunner)_domain.CreateInstanceFromAndUnwrap(dll, "Pipeline.Desktop.Transforms.CompilerRunner", false, BindingFlags.Default, null, null, null, null)) {
                var lease = compilerRunner.InitializeLifetimeService() as ILease;
                lease?.Register(_sponsor);

                var timer = new Stopwatch();
                timer.Start();

                var errors = compilerRunner.Compile(code);
                if (string.IsNullOrEmpty(errors)) {
                    Context.Info($"Compiled in {timer.Elapsed}");
                    timer.Stop();
                } else {
                    Context.Error(errors);
                    Context.Error(Context.Transform.Script.Replace("{", "{{").Replace("}", "}}"));
                }

                foreach (var row in rows) {
                    row[Context.Field] = compilerRunner.Run(row.ToArray());
                    Increment();
                    yield return row;
                }
            }
        }

        public override void Dispose() {
            if (_sponsor != null) {
                Context.Info("Release lease.");
                _sponsor.Release = true;
            }
            if (_domain != null) {
                Context.Info("Unload app domain.");
                AppDomain.Unload(_domain);
            }
            base.Dispose();
        }

        public static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }


    class Sponsor : MarshalByRefObject, ISponsor {
        public bool Release { get; set; }

        public TimeSpan Renewal(ILease lease) {
            // if any of these cases is true
            if (lease == null || lease.CurrentState != LeaseState.Renewing || Release)
                return TimeSpan.Zero; // don't renew
            return TimeSpan.FromSeconds(5); // renew for 5 seconds
        }
    }

    public class CompilerRunner : MarshalByRefObject, IDisposable {

        private Func<object[], object> _userCode;

        public string Compile(string code) {

            var codeProvider = new Microsoft.CSharp.CSharpCodeProvider();
            var parameters = new CompilerParameters {
                GenerateInMemory = true,
                GenerateExecutable = false
            };

            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("mscorlib.dll");
            parameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);

            var sb = new StringBuilder();

            try {
                var result = codeProvider.CompileAssemblyFromSource(parameters, code);
                if (result.Errors.Count > 0) {
                    sb.AppendLine("CSharp Compiler Error!");
                    foreach (var error in result.Errors) {
                        sb.AppendLine(error.ToString());
                    }
                } else {
                    _userCode = (Func<object[], object>)Delegate.CreateDelegate(typeof(Func<object[], object>), result.CompiledAssembly.GetType("CSharpRunTimeTransform").GetMethod("UserCode", BindingFlags.Static | BindingFlags.Public));
                }
            } catch (Exception ex) {
                sb.AppendLine("CSharp Compiler Exception!");
                sb.AppendLine(ex.Message);
            }

            return sb.ToString();
        }

        public object Run(object[] data) {
            return _userCode(data);
        }

        public override object InitializeLifetimeService() {
            var lease = base.InitializeLifetimeService() as ILease;
            if (lease == null || lease.CurrentState != LeaseState.Initial)
                return lease;

            lease.InitialLeaseTime = TimeSpan.FromSeconds(10);
            lease.SponsorshipTimeout = TimeSpan.FromSeconds(10);
            lease.RenewOnCallTime = TimeSpan.FromSeconds(5);
            return lease;
        }

        public void Dispose() {
            _userCode = null;
        }
    }
}