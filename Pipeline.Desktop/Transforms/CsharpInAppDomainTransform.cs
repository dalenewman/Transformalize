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
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using Pipeline.Contracts;
using Pipeline.Transforms;

namespace Pipeline.Desktop.Transforms {

    public class CsharpInAppDomainTransform : BaseTransform {

        private readonly AppDomain _domain;
        private readonly CompilerRunner _compilerRunner;

        public CsharpInAppDomainTransform(IContext context) : base(context) {

            var timer = new Stopwatch();
            timer.Start();

            var input = MultipleInput();
            var sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine();

            sb.AppendLine("public class CSharpRunTimeTransform {");

            sb.AppendLine("    public static object UserCode(object[] data) {");

            for (var i = 0; i < input.Length; i++) {
                var field = input[i];
                var objectIndex = Context.Entity.IsMaster ? field.MasterIndex : field.Index;
                string type;
                switch (field.Type) {
                    case "date":
                    case "datetime":
                        type = "DateTime";
                        break;
                    default:
                        type = field.Type;
                        break;
                }
                sb.AppendLine($"        {type} {Pipeline.Utility.Identifier(field.Alias)} = ({type}) data[{objectIndex}];");
            }

            sb.Append("        ");
            // handles csharp body or an expression
            sb.AppendLine(context.Transform.Script.Contains("return ") ? context.Transform.Script : "return " + (context.Transform.Script.EndsWith(";") ? context.Transform.Script : context.Transform.Script + ";"));

            sb.AppendLine();
            sb.AppendLine("    }");
            sb.AppendLine("}");

            var code = sb.ToString();
            context.Debug((() => code));

            context.Info("Creating new app domain.");
            _domain = AppDomain.CreateDomain(context.Key);
            _compilerRunner = (CompilerRunner)_domain.CreateInstanceFromAndUnwrap("Pipeline.Desktop.dll", "Pipeline.Desktop.Transforms.CompilerRunner");

            var errors = _compilerRunner.Compile(code);
            if (string.IsNullOrEmpty(errors)) {
                context.Info($"Compiled in {timer.Elapsed}");
            } else {
                context.Error(errors);
                context.Error(context.Transform.Script.Replace("{", "{{").Replace("}", "}}"));
            }

            timer.Stop();
        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = _compilerRunner.Run(row.ToArray());
            Increment();
            return row;
        }

        public override void Dispose() {
            Context.Info("Unload app domain.");
            AppDomain.Unload(_domain);
            base.Dispose();
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

                    //_userCode = (data)=> methodInfo.Invoke(null, new object[] { data });
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

            lease.InitialLeaseTime = TimeSpan.FromMinutes(5);
            lease.SponsorshipTimeout = TimeSpan.FromMinutes(5);
            lease.RenewOnCallTime = TimeSpan.FromMinutes(5);
            return lease;
        }

    }
}