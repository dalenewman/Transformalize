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

namespace Transformalize.Main
{
    public class ConversionFactory
    {
        public object Convert(object something, string type)
        {
            var simpleType = Common.ToSimpleType(type);
            if (simpleType == "object")
                return something;

            var value = something == null ? string.Empty : something.ToString();
            switch (simpleType)
            {
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
                case "byte[]":
                    return value == string.Empty ? new byte[0] : something;
                case "rowversion":
                    return value == string.Empty ? new byte[0] : something;
                default:
                    return value;
            }
        }
    }
}