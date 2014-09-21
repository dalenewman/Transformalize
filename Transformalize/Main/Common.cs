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
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Transformalize.Configuration;
using Transformalize.Extensions;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using Transformalize.Libs.NLog;

namespace Transformalize.Main {
    public static class Common {

        private static readonly Logger Log = LogManager.GetLogger("tfl");
        private const string APPLICATION_FOLDER = @"\Tfl\";
        private static readonly char[] Slash = { '\\' };
        private const string CLEAN_PATTERN = @"[^\w]";

        public const string DefaultValue = "[default]";

        public static string GuardTimeZone(string timeZone, string defaultTimeZone) {
            var result = timeZone;
            if (timeZone == String.Empty) {
                result = defaultTimeZone;
                Log.Debug("Defaulting From TimeZone to {0}.", defaultTimeZone);
            } else {
                if (!TimeZoneInfo.GetSystemTimeZones().Any(tz => tz.Id.Equals(timeZone))) {
                    throw new TransformalizeException("From Timezone Id {0} is invalid.", timeZone);
                }
            }
            return result;
        }

        public static Dictionary<string, byte> Validators = new Dictionary<string, byte> {
            {"containscharacters", 1},
            {"datetimerange", 1},
            {"domain", 1},
            {"isjson", 1},
            {"notnull", 1},
            {"fieldcomparison", 1},
            {"range", 1},
            {"regex", 1},
            {"relativedatetime", 1},
            {"stringlength", 1},
            {"typeconversion", 1}
        };

        public static bool IsValidator(string method) {
            return Validators.ContainsKey(method.ToLower());
        }



        public static Dictionary<string, byte> TypeMap = new Dictionary<string, byte> {
            {"string", 0},
            {"xml", 0},
            {"int16", 1},
            {"short", 1},
            {"int32", 2},
            {"int", 2},
            {"int64", 3},
            {"long", 3},
            {"double", 4},
            {"decimal", 5},
            {"char", 6},
            {"datetime", 7},
            {"date", 7},
            {"boolean", 8},
            {"bool", 8},
            {"single", 9},
            {"real", 9},
            {"float", 9},
            {"guid", 10},
            {"byte", 11},
            {"byte[]", 12},
            {"rowversion", 12},
            {"uint64", 13},
            {"object", 14}
        };

        public static Dictionary<string, Func<string, object>> ConversionMap = new Dictionary<string, Func<string, object>> {
            {"string", (x => x)},
            {"xml", (x => x)},
            {"int16", (x => Convert.ToInt16(x))},
            {"short", (x => Convert.ToInt16(x))},
            {"int32", (x => Convert.ToInt32(x))},
            {"int", (x => Convert.ToInt32(x))},
            {"int64", (x => Convert.ToInt64(x))},
            {"long", (x => Convert.ToInt64(x))},
            {"uint64", (x => Convert.ToUInt64(x))},
            {"double", (x => Convert.ToDouble(x))},
            {"decimal", (x => Decimal.Parse(x, NumberStyles.Float | NumberStyles.AllowThousands | NumberStyles.AllowCurrencySymbol, (IFormatProvider)CultureInfo.CurrentCulture.GetFormat(typeof(NumberFormatInfo))))},
            {"char", (x => Convert.ToChar(x))},
            {"datetime", (x => Convert.ToDateTime(x))},
            {"boolean", (x => Convert.ToBoolean(x))},
            {"single", (x => Convert.ToSingle(x))},
            {"real", (x => Convert.ToSingle(x))},
            {"float", (x => Convert.ToSingle(x))},
            {"guid", (x => Guid.Parse(x))},
            {"byte", (x => Convert.ToByte(x))},
            {"byte[]", (HexStringToByteArray)},
            {"rowversion", (HexStringToByteArray)}
        };

        public static Dictionary<ComparisonOperator, Func<object, object, bool>> CompareMap = new Dictionary<ComparisonOperator, Func<object, object, bool>>() {
            {ComparisonOperator.Equal, ((x, y) => x.Equals(y))},
            {ComparisonOperator.NotEqual, ((x, y) => !x.Equals(y))},
            {ComparisonOperator.GreaterThan, ((x, y) => ((IComparable) x).CompareTo(y) > 0)},
            {ComparisonOperator.GreaterThanEqual, ((x, y) => x.Equals(y) || ((IComparable) x).CompareTo(y) > 0)},
            {ComparisonOperator.LessThan, ((x, y) => ((IComparable) x).CompareTo(y) < 0)},
            {ComparisonOperator.LessThanEqual, ((x, y) => x.Equals(y) || ((IComparable) x).CompareTo(y) < 0)}
        };

        public static Dictionary<string, Func<object, string>> GetLiteral() {
            return new Dictionary<string, Func<object, string>> {
                {"string", (x => String.Format("\"{0}\"",x))},
                {"xml", (x => String.Format("\"{0}\"",x))},
                {"guid", (x => String.Format("Guid.Parse(\"{0}\")",x))},
                {"int16", (x => x.ToString())},
                {"int", (x => x.ToString())},
                {"int32", (x => x.ToString())},
                {"short", (x => x.ToString())},
                {"int64", (x => x.ToString())},
                {"uint64", (x => x.ToString())},
                {"long", (x => x.ToString() + "L")},
                {"double", (x => x.ToString() + "D")},
                {"decimal", (x => x.ToString() + "M")},
                {"char", (x => String.Format("'{0}'",x))},
                {"datetime", (x => String.Format("Convert.ToDateTime(\"{0}\")",x))},
                {"boolean", (x => x.ToString().ToLower())},
                {"bool", (x => x.ToString().ToLower())},
                {"single", (x => x.ToString())},
                {"float", (x => x.ToString()+"F")},
                {"real", (x => x.ToString())},
                {"byte", (x => x.ToString())},
                {"byte[]", (x => String.Format("Common.HexStringToByteArray(\"{0}\")",x))},
                {"rowversion", (x => String.Format("Common.HexStringToByteArray(\"{0}\")",x))}
            };
        }

        public static Dictionary<string, Func<object, object>> GetObjectConversionMap() {
            return new Dictionary<string, Func<object, object>> {
                {"string", (x => x)},
                {"xml", (x => x)},
                {"guid", (x => Guid.Parse(x.ToString()))},
                {"int16", (x => Convert.ToInt16(x))},
                {"short", (x => Convert.ToInt16(x))},
                {"int", (x => Convert.ToInt32(x))},
                {"int32", (x => Convert.ToInt32(x))},
                {"int64", (x => Convert.ToInt64(x))},
                {"uint64", (x=> Convert.ToUInt64(x))},
                {"long", (x => Convert.ToInt64(x))},
                {"double", (x => Convert.ToDouble(x))},
                {"decimal", (x => Decimal.Parse(x.ToString(), NumberStyles.Float | NumberStyles.AllowThousands | NumberStyles.AllowCurrencySymbol, (IFormatProvider)CultureInfo.CurrentCulture.GetFormat(typeof(NumberFormatInfo))))},
                {"char", (x => Convert.ToChar(x))},
                {"datetime", (x => Convert.ToDateTime(x))},
                {"boolean", (x => Convert.ToBoolean(x))},
                {"float", (x => Convert.ToSingle(x))},
                {"bool", (x => Convert.ToBoolean(x))},
                {"real", (x => Convert.ToSingle(x))},
                {"single", (x => Convert.ToSingle(x))},
                {"byte", (x => Convert.ToByte(x))},
                {"byte[]", (x => HexStringToByteArray(x.ToString()))},
                {"rowversion", (x => HexStringToByteArray(x.ToString()))}
            };
        }

        public static string GetAlias(FieldConfigurationElement element, bool usePrefix, string prefix) {
            return usePrefix && element.Alias.Equals(element.Name) && !String.IsNullOrEmpty(prefix) ? prefix + element.Name : element.Alias;
        }

        public static string GetTemporaryFolder(string processName) {
            var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).TrimEnd(Slash);

            //i.e. c: no user profile exists
            if (local.Length <= 2) {
                if (AppDomain.CurrentDomain.GetData("DataDirectory") != null) {
                    local = AppDomain.CurrentDomain.GetData("DataDirectory").ToString().TrimEnd(Slash);
                }
            }

            var folder = local + APPLICATION_FOLDER + processName;

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            return folder;
        }

        public static string GetTemporarySubFolder(string processName, string subFolder) {
            var f = Path.Combine(GetTemporaryFolder(processName), subFolder);
            if (!Directory.Exists(f)) {
                Directory.CreateDirectory(f);
            }
            return f;
        }

        public static byte[] HexStringToByteArray(string hex) {
            var bytes = new byte[hex.Length / 2];
            var hexValue = new[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };

            for (int x = 0, i = 0; i < hex.Length; i += 2, x += 1) {
                bytes[x] = (byte)(hexValue[Char.ToUpper(hex[i + 0]) - '0'] << 4 |
                                  hexValue[Char.ToUpper(hex[i + 1]) - '0']);
            }

            return bytes;
        }

        public static string BytesToHexString(byte[] bytes) {
            var c = new char[bytes.Length * 2];
            for (var i = 0; i < bytes.Length; i++) {
                var b = bytes[i] >> 4;
                c[i * 2] = (char)(55 + b + (((b - 10) >> 31) & -7));
                b = bytes[i] & 0xF;
                c[i * 2 + 1] = (char)(55 + b + (((b - 10) >> 31) & -7));
            }
            return new string(c);
        }

        public static string ToSimpleType(string type) {
            var result = type.ToLower();
            if (result == "int") {
                result = "int32";
            }
            if (result == "long") {
                result = "int64";
            }
            if (result == "short") {
                result = "int16";
            }
            if (result == "real") {
                result = "single";
            }
            if (result == "bool") {
                result = "boolean";
            }
            return result.Replace("system.", String.Empty);
        }

        public static Type ToSystemType(string simpleType) {
            simpleType = ToSimpleType(simpleType);
            switch (simpleType) {
                case "byte[]":
                    return typeof(byte[]);
                case "rowversion":
                    return typeof(byte[]);
                case "datetime":
                    return typeof(DateTime);
                case "uint16":
                    return typeof(UInt16);
                case "uint32":
                    return typeof(UInt32);
                case "uint64":
                    return typeof(UInt64);
                default:
                    var fullName = "System." + simpleType[0].ToString(CultureInfo.InvariantCulture).ToUpper() + simpleType.Substring(1);
                    return Type.GetType(fullName) ?? Type.GetType(simpleType);
            }
        }

        public static int DateTimeToInt32(DateTime date) {
            return (int)(date - new DateTime(1, 1, 1)).TotalDays + 1;
        }

        public static DateTime Int32ToDateTime(int timeKey) {
            return new DateTime(1, 1, 1).AddDays(timeKey - 1);
        }

        public static string LogLength(string value, int totalWidth = 20) {
            return value.Length > totalWidth ? value.Left(totalWidth) : value.PadRight(totalWidth, '.');
        }

        public static string EntityOutputName(Entity entity, string processName) {
            return entity.PrependProcessNameToOutputName ? String.Concat(processName.Replace("-", String.Empty), entity.Alias).Replace(" ", String.Empty) : entity.Alias;
        }

        public static bool AreEqual(byte[] b1, byte[] b2) {
            return b1.Length == b2.Length && b1.SequenceEqual(b2);
        }

        public static string CleanIdentifier(string input) {
            var sb = new StringBuilder(Regex.Replace(input, CLEAN_PATTERN, "_"));
            sb.Push(c => c.Equals('_') || Char.IsNumber(c));
            sb.Trim(" ");
            var result = sb.ToString();
            if (result.Equals(String.Empty) || result.All(c => c.Equals('_') || Char.IsNumber(c))) {
                result = "I" + input.GetHashCode().ToString(CultureInfo.InvariantCulture).Replace("-", "0");
            }
            if (!input.Equals(result)) {
                Log.Debug("Using '{0}' to identify field '{1}'.", result, input);
            }
            return sb.ToString();
        }

        public static string[] Split(string arg, string splitter, int skip = 0) {
            if (arg.Equals(String.Empty))
                return new string[0];

            var placeHolder = arg.GetHashCode().ToString(CultureInfo.InvariantCulture);
            var split = arg.Replace("\\" + splitter, placeHolder).Split(new string[] { splitter }, StringSplitOptions.None);
            return split.Select(s => s.Replace(placeHolder, splitter.ToString(CultureInfo.InvariantCulture))).Skip(skip).ToArray();
        }
    }
}