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
using System.Text;
using Pipeline.Contracts;

namespace Pipeline.Desktop.Transforms {

    /// <summary>
    /// This transform is known to be leaky and should not be used in long-lived services.  However, 
    /// it should be faster than the CSharpRemoteTransform if your application is short-lived (i.e. during 'init' mode).
    /// </summary>
    public class CsharpLocalTransform : CSharpBaseTransform {

        private readonly Func<object[], object> _userCode;

        public CsharpLocalTransform(IContext context) : base(context) {

            var timer = new Stopwatch();
            timer.Start();

            var input = MultipleInput();
            var code = WrapCode(input, Context.Transform.Script, context.Entity.IsMaster);
            context.Debug((() => code));


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
                    context.Error("CSharp Compiler Error!");
                    foreach (var error in result.Errors) {
                        context.Error(error.ToString());
                    }
                    context.Error(context.Transform.Script.Replace("{", "{{").Replace("}", "}}"));
                } else {
                    var type = result.CompiledAssembly.GetType("CSharpRunTimeTransform");
                    var methodInfo = type.GetMethod("UserCode", BindingFlags.Static | BindingFlags.Public);
                    _userCode = (Func<object[], object>)Delegate.CreateDelegate(typeof(Func<object[], object>), methodInfo);
                    context.Info($"Compiled in {timer.Elapsed}");
                }
            } catch (Exception ex) {
                sb.AppendLine("CSharp Compiler Exception!");
                sb.AppendLine(ex.Message);
                context.Error(context.Transform.Script.Replace("{", "{{").Replace("}", "}}"));
            }

            timer.Stop();
        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = _userCode(row.ToArray());
            Increment();
            return row;
        }

    }

}