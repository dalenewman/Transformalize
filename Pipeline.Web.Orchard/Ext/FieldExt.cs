using System.Collections.Generic;
using System.Linq;
using Field = Transformalize.Configuration.Field;

namespace Pipeline.Web.Orchard.Ext {
    public static class FieldExt {
        public static string ToParsley(this Field f) {
            if (f.V == string.Empty)
                return string.Empty;

            var attributes = new Dictionary<string, string>();

            var expressions = new Cfg.Net.Shorthand.Expressions(f.V);
            foreach (var expression in expressions) {
                switch (expression.Method) {
                    case "required":
                        if (f.InputType == "file") {
                            continue;
                        }
                        attributes["data-parsley-required"] = "true";
                        break;
                    case "length":
                        attributes["data-parsley-length"] = $"[{expression.SingleParameter}, {expression.SingleParameter}]";
                        break;
                    case "numeric":
                        attributes["data-parsley-type"] = "number";
                        break;
                    case "min":
                        attributes["data-parsley-min"] = expression.SingleParameter;
                        break;
                    case "max":
                        attributes["data-parsley-max"] = expression.SingleParameter;
                        break;
                    case "is":
                        switch (expression.SingleParameter) {
                            case "int":
                            case "int32":
                                attributes["data-parsley-type"] = "integer";
                                break;
                            case "date":
                            case "datetime":
                                attributes["data-parsley-date"] = "true";
                                break;
                        }
                        break;
                    case "alphanum":
                        attributes["data-parsley-type"] = "alphanum";
                        break;
                }
            }


            return string.Join(" ",attributes.Select(i => $"{i.Key}=\"{i.Value}\""));
        }
    }
}