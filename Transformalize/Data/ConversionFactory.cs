using System;

namespace Transformalize.Data {
    public class ConversionFactory {

        public object Convert(object something, string type) {

            var simpleType = type.ToLower().Replace("system.", string.Empty);
            if (simpleType == "object")
                return something;

            var value = something.ToString();
            switch (simpleType) {
                case "datetime":
                    if (value == string.Empty)
                        value = "9999-12-31";
                    return System.Convert.ToDateTime(value);
                case "boolean":
                    if (value == string.Empty)
                        value = "false";
                    if (value == "0")
                        value = "false";
                    if (value == "1")
                        value = "true";
                    return System.Convert.ToBoolean(value);
                case "decimal":
                    if (value == string.Empty)
                        value = "0.0";
                    return System.Convert.ToDecimal(value);
                case "double":
                    if (value == string.Empty)
                        value = "0.0";
                    return System.Convert.ToDouble(value);
                case "single":
                    if (value == string.Empty)
                        value = "0.0";
                    return System.Convert.ToSingle(value);
                case "int64":
                    if (value == string.Empty)
                        value = "0";
                    return System.Convert.ToInt64(value);
                case "int32":
                    if (value == string.Empty)
                        value = "0";
                    return System.Convert.ToInt32(value);
                case "int16":
                    if (value == string.Empty)
                        value = "0";
                    return System.Convert.ToInt16(value);
                case "byte":
                    if (value == string.Empty)
                        value = "0";
                    return System.Convert.ToByte(value);
                case "guid":
                    if (value == string.Empty)
                        value = "00000000-0000-0000-0000-000000000000";
                    return value.ToLower() == "new" ? Guid.NewGuid() : Guid.Parse(value);
                case "char":
                    if (value == string.Empty)
                        value = " ";
                    return System.Convert.ToChar(value);
                default:
                    return value;
            }

        }

    }
}
