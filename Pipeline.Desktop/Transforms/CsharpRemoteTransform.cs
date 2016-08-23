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
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using Pipeline.Contracts;

namespace Pipeline.Desktop.Transforms {

    public class CsharpRemoteTransform : CSharpBaseTransform {

        private AppDomain _domain;
        private CompilerRunner _compilerRunner;
        private Sponsor _sponsor;

        public CsharpRemoteTransform(IContext context) : base(context) {

        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = _compilerRunner.Run(row.ToArray());
            Increment();
            return row;
        }

        public override IEnumerable<IRow> Transform(IEnumerable<IRow> rows) {
            var timer = new Stopwatch();
            timer.Start();

            var input = MultipleInput();
            var code = WrapCode(input, Context.Transform.Script, Context.Entity.IsMaster);

            Context.Debug((() => code));

            Context.Info("Creating new app domain and sponsor.");
            _domain = AppDomain.CreateDomain(Context.Key);

            _sponsor = new Sponsor();  // manages lifetime of the object in otherDomain
            _compilerRunner = (CompilerRunner)_domain.CreateInstanceFromAndUnwrap("Pipeline.Desktop.dll", "Pipeline.Desktop.Transforms.CompilerRunner");

            var lease = _compilerRunner.InitializeLifetimeService() as ILease;
            lease?.Register(_sponsor);

            var errors = _compilerRunner.Compile(code);
            if (string.IsNullOrEmpty(errors)) {
                Context.Info($"Compiled in {timer.Elapsed}");
            } else {
                Context.Error(errors);
                Context.Error(Context.Transform.Script.Replace("{", "{{").Replace("}", "}}"));
            }

            timer.Stop();

            return base.Transform(rows);
        }

        public override void Dispose() {
            Context.Info("Release lease and unload app domain.");
            if (_sponsor != null) {
                _sponsor.Release = true;
            }
            if (_domain != null) {
                AppDomain.Unload(_domain);
            }
            base.Dispose();
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

    public class CompilerRunner : MarshalByRefObject {

        private Assembly _assembly;
        private Type _type;
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
                    _assembly = result.CompiledAssembly;
                    _type = _assembly.GetType("CSharpRunTimeTransform");
                    var methodInfo = _type.GetMethod("UserCode", BindingFlags.Static | BindingFlags.Public);

                    _userCode = (Func<object[], object>)Delegate.CreateDelegate(typeof(Func<object[], object>), methodInfo);
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

    }
}