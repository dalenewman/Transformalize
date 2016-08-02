using System.Text;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Transforms;

namespace Pipeline.Desktop.Transforms {
    public class CSharpBaseTransform : BaseTransform {
        public CSharpBaseTransform(IContext context) : base(context) {
        }

        public override IRow Transform(IRow row) {
            return row;
        }

        public static string WrapCode(Field[] input, string code, bool isMaster) {
            var sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine();

            sb.AppendLine("public class CSharpRunTimeTransform {");

            sb.AppendLine("    public static object UserCode(object[] data) {");

            for (var i = 0; i < input.Length; i++) {
                var field = input[i];
                var objectIndex = isMaster ? field.MasterIndex : field.Index;
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
            sb.AppendLine(code.Contains("return ") ? code : "return " + (code.EndsWith(";") ? code : code + ";"));
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();

        }

    }
}