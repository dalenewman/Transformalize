#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;

namespace Transformalize.Main {
    public class DefaultFactory {

        public object Convert(object something, string type) {
            var simpleType = Common.ToSimpleType(type);
            if (simpleType == "object")
                return something;

            string value;

            if (something == null) {
                value = string.Empty;
            } else {
                if (type == "byte[]" || type == "rowversion") {
                    value = something.ToString() == string.Empty ? Common.BytesToHexString(new byte[0]) : Common.BytesToHexString((byte[])something);
                } else {
                    value = something.ToString();
                }
            }

            try {
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
                    case "int":
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
                    case "byte[]":
                        if (value == string.Empty)
                            value = Common.BytesToHexString(new byte[0]);
                        return Common.HexStringToByteArray(value);
                    case "rowversion":
                        if (value == string.Empty)
                            value = Common.BytesToHexString(new byte[0]);
                        return Common.HexStringToByteArray(value);
                    default:
                        return value;
                }

            } catch (Exception e) {
                throw new TransformalizeException("Trouble converting {0} to {1}. {2}", value, type, e.Message);
            }

        }

        public object Convert(string value, string type, object def) {
            value = value ?? string.Empty;
            var simpleType = Common.ToSimpleType(type);
            if (simpleType == "object")
                return value;

            switch (simpleType) {
                case "string":
                    return value == string.Empty ? def : value;
                case "datetime":
                    return value == string.Empty ? def : System.Convert.ToDateTime(value);
                case "boolean":
                    if (value == string.Empty)
                        return def;
                    if (value == "0")
                        value = "false";
                    if (value == "1")
                        value = "true";
                    return System.Convert.ToBoolean(value);
                case "decimal":
                    return value == string.Empty ? def : System.Convert.ToDecimal(value);
                case "double":
                    return value == string.Empty ? def : System.Convert.ToDouble(value);
                case "single":
                    return value == string.Empty ? def : System.Convert.ToSingle(value);
                case "int64":
                    return value == string.Empty ? def : System.Convert.ToInt64(value);
                case "int32":
                    if (value == string.Empty)
                        value = "0";
                    return System.Convert.ToInt32(value);
                case "int":
                    return value == string.Empty ? def : System.Convert.ToInt32(value);
                case "int16":
                    return value == string.Empty ? def : System.Convert.ToInt16(value);
                case "byte":
                    return value == string.Empty ? def : System.Convert.ToByte(value);
                case "guid":
                    if (value == string.Empty)
                        return def;
                    return value.ToLower() == "new" ? Guid.NewGuid() : Guid.Parse(value);
                case "char":
                    return value == string.Empty ? def : System.Convert.ToChar(value);
                case "byte[]":
                    return value == string.Empty ? def : Common.HexStringToByteArray(value);
                case "rowversion":
                    return value == string.Empty ? def : Common.HexStringToByteArray(value);
                default:
                    return value;
            }
        }

    }
}