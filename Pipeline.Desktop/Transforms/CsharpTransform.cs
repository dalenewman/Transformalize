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
using System.IO;
using System.Reflection;
using System.Text;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Nulls;
using Pipeline.Transforms;

namespace Pipeline.Desktop.Transforms {

    public class CsharpTransform : BaseTransform {

        private readonly ITransform _transform;
        public CsharpTransform(IContext context) : base(context) {

            var timer = new Stopwatch();
            timer.Start();
            var input = MultipleInput();

            var compiler = new Microsoft.CSharp.CSharpCodeProvider();

            var assembly = Assembly.GetExecutingAssembly();
            var parameters = new CompilerParameters { GenerateInMemory = true };
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("System.Runtime.dll");
            parameters.ReferencedAssemblies.Add("mscorlib.dll");
            parameters.ReferencedAssemblies.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pipeline.Portable.dll"));
            parameters.ReferencedAssemblies.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CfgNet.dll"));
            parameters.ReferencedAssemblies.Add(assembly.Location);

            var code = new StringBuilder();

            code.AppendLine("using System;");
            code.AppendLine();

            code.AppendLine("public class CSharpRunTimeTransform : Pipeline.Transforms.BaseTransform {");

            code.AppendLine(@"private readonly Pipeline.Configuration.Field[] _input;
    public CSharpRunTimeTransform(Pipeline.Contracts.IContext context) : base(context)     {
        _input = MultipleInput();
    }");
            code.AppendLine("    public object UsersCode(Pipeline.Contracts.IRow row) {");

            for (var i = 0; i < input.Length; i++) {
                var field = input[i];
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
                code.AppendLine($"        {type} {Pipeline.Utility.Identifier(field.Alias)} = ({type}) row[_input[{i}]];");
            }

            // handles csharp body or an expression
            code.AppendLine(context.Transform.Script.Contains("return ") ? context.Transform.Script : "return " + (context.Transform.Script.EndsWith(";") ? context.Transform.Script : context.Transform.Script + ";"));
            code.AppendLine();

            code.AppendLine("}");

            code.AppendLine("public override Pipeline.Contracts.IRow Transform(Pipeline.Contracts.IRow row) {");

            code.AppendLine("row[Context.Field] = UsersCode(row);");
            code.AppendLine("Increment();");
            code.AppendLine("return row;");


            code.AppendLine("}");
            code.AppendLine("}");

            var complete = code.ToString();
            context.Debug((() => complete));

            try {
                var result = compiler.CompileAssemblyFromSource(parameters, complete);
                if (result.Errors.Count == 0) {
                    var type = result.CompiledAssembly.GetType("CSharpRunTimeTransform");
                    _transform = (ITransform)Activator.CreateInstance(type, context);
                } else {
                    context.Error($"CSharp Compiler Error in {Context.Field.Alias}.");
                    foreach (var error in result.Errors) {
                        context.Error(error.ToString());
                    }
                    context.Error(Context.Transform.Script.Replace("{", "{{").Replace("}", "}}"));
                    _transform = new NullTransform(context);
                }
            } catch (Exception ex) {
                context.Error($"CSharp Compiler Exception in {Context.Field.Alias}.");
                context.Error(ex.Message);
                context.Error(Context.Transform.Script.Replace("{", "{{").Replace("}", "}}"));
                _transform = new NullTransform(context);
            }

            timer.Stop();
            context.Info($"Compiled in {timer.Elapsed}");
        }

        public override IRow Transform(IRow row) {
            return _transform.Transform(row);
        }
    }

}