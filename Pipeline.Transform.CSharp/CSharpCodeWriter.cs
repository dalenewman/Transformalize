#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Transform.CSharp {

    public interface IWriteSomething {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">A name for your code.  Could be a class name.</param>
        /// <returns></returns>
        string Write(string name);
    }

    public class CSharpCodeWriter : IWriteSomething {
        private readonly IContext _context;

        public CSharpCodeWriter(IContext context) {
            _context = context;
        }

        public string Write(string name) {
            var sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine();

            sb.AppendLine($"public class {name} {{");

            foreach (var entity in _context.Process.Entities) {
                foreach (var field in entity.Fields.Where(f => f.Transforms.Any(t => t.Method.In("cs", "csharp")))) {
                    WriteMethods(_context.Process, entity, field, sb, _context.Logger);
                }
                foreach (var field in entity.CalculatedFields.Where(f => f.Transforms.Any(t => t.Method.In("cs", "csharp")))) {
                    WriteMethods(_context.Process, entity, field, sb, _context.Logger);
                }
            }

            // calculated fields are from an internally created process
            var calcProcess = _context.Process.ToCalculatedFieldsProcess();
            var calcEntity = calcProcess.Entities.First();
            foreach (var field in calcEntity.CalculatedFields.Where(f => f.Transforms.Any(t => t.Method.In("cs", "csharp")))) {
                WriteMethods(calcProcess, calcEntity, field, sb, _context.Logger);
            }

            sb.Append("}");
            var code = sb.ToString();
            _context.Debug(() => code);
            return code;
        }

        /// <summary>
        /// Hides the work of creating the context and input for each method
        /// </summary>
        /// <param name="process"></param>
        /// <param name="entity"></param>
        /// <param name="field"></param>
        /// <param name="sb"></param>
        /// <param name="logger"></param>
        private static void WriteMethods(Process process, Entity entity, Field field, StringBuilder sb, IPipelineLogger logger) {
            foreach (var transform in field.Transforms.Where(t => t.Method == "cs" || t.Method == "csharp")) {
                var tc = new PipelineContext(logger, process, entity, field, transform);
                var input = process.ParametersToFields(transform.Parameters, field);
                WriteMethod(tc, input, sb);
            }
        }

        /// <summary>
        /// Writes the method according to specific context and input
        /// </summary>
        /// <param name="tc"></param>
        /// <param name="input"></param>
        /// <param name="sb"></param>
        private static void WriteMethod(IContext tc, IEnumerable<Field> input, StringBuilder sb) {
            var methodName = Utility.GetMethodName(tc);
            sb.AppendLine($"    public static object {methodName}(object[] data) {{");

            foreach (var field in input) {

                var objectIndex = tc.Entity.IsMaster ? field.MasterIndex : field.Index;
                string type;
                switch (field.Type) {
                    case "date":
                    case "datetime":
                        type = "DateTime";
                        break;
                    case "single":
                    case "int16":
                    case "int32":
                    case "int64":
                        type = field.Type.Left(1).ToUpper() + field.Type.Substring(1);
                        break;
                    default:
                        type = field.Type;
                        break;
                }
                sb.AppendLine($"        {type} {Utility.Identifier(field.Alias)} = ({type}) data[{objectIndex}];");
            }

            sb.Append("        ");
            // handles csharp body or an expression
            sb.AppendLine(tc.Transform.Script.Contains("return ") ? tc.Transform.Script : "return " + (tc.Transform.Script.EndsWith(";") ? tc.Transform.Script : tc.Transform.Script + ";"));
            sb.AppendLine("    }");

        }

    }
}