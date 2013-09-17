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
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using Transformalize.Configuration;
using Transformalize.Extensions;

namespace Transformalize.Main {
    public static class Common {
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private const string APPLICATION_FOLDER = @"\Tfl\";
        private static readonly char[] Slash = new[] { '\\' };

        public static Dictionary<string, Func<string, object>> ConversionMap = new Dictionary<string, Func<string, object>> {
            {"string", (x => x)},
            {"xml", (x => x)},
            {"int16", (x => Convert.ToInt16(x))},
            {"int32", (x => Convert.ToInt32(x))},
            {"int64", (x => Convert.ToInt64(x))},
            {"long", (x => Convert.ToInt64(x))},
            {"double", (x => Convert.ToDouble(x))},
            {"decimal", (x => Convert.ToDecimal(x))},
            {"char", (x => Convert.ToChar(x))},
            {"datetime", (x => Convert.ToDateTime(x))},
            {"boolean", (x => Convert.ToBoolean(x))},
            {"single", (x => Convert.ToSingle(x))}
        };

        public static Dictionary<string, Func<object, object>> ObjectConversionMap = new Dictionary<string, Func<object, object>> {
            { "string", (x => x) },
            { "xml", (x => x) },
            { "int16", (x => Convert.ToInt16(x)) },
            { "int32", (x => Convert.ToInt32(x)) },
            { "int64", (x => Convert.ToInt64(x)) },
            { "long", (x => Convert.ToInt64(x)) },
            { "double", (x => Convert.ToDouble(x)) },
            { "decimal", (x => Convert.ToDecimal(x)) },
            { "char", (x => Convert.ToChar(x)) },
            { "datetime", (x => Convert.ToDateTime(x)) },
            { "boolean", (x => Convert.ToBoolean(x)) },
            { "single", (x => Convert.ToSingle(x)) }
        };

        public static Func<KeyValuePair<string, Field>, bool> FieldFinder(ParameterConfigurationElement p) {
            if (p.Entity != string.Empty)
                return f => f.Key.Equals(p.Field, IC) && f.Value.Entity.Equals(p.Entity, IC) || f.Value.Name.Equals(p.Field, IC) && f.Value.Entity.Equals(p.Entity, IC);
            return f => f.Key.Equals(p.Field, IC) || f.Value.Name.Equals(p.Field, IC);
        }

        public static Func<Field, bool> FieldFinder(string nameOrAlias) {
            return v => v.Name.Equals(nameOrAlias, IC) || v.Alias.Equals(nameOrAlias, IC);
        }

        public static string GetTemporaryFolder(string processName) {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).TrimEnd(Slash) + APPLICATION_FOLDER + processName;

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            return folder;
        }

        public static IEnumerable<byte> ObjectToByteArray(object obj) {
            if (obj == null)
                return null;
            var formatter = new BinaryFormatter();
            var memory = new MemoryStream();
            formatter.Serialize(memory, obj);
            return memory.ToArray();
        }

        public static string ToSimpleType(string type) {
            return type.ToLower().Replace("system.", string.Empty);
        }

        public static int DateTimeToInt32(DateTime date) {
            return (int)(date - new DateTime(1, 1, 1)).TotalDays + 1;
        }

        public static DateTime Int32ToDateTime(int timeKey) {
            return new DateTime(1, 1, 1).AddDays(timeKey - 1);
        }

        public static string LogLength(string value, int totalWidth) {
            return value.Length > totalWidth ? value.Left(totalWidth) : value.PadRight(totalWidth, '.');
        }

        public static string EntityOutputName(string entityAlias, string processName)
        {
            return string.Concat(processName, entityAlias).Replace(" ", string.Empty);
        }

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);

        public static bool AreEqual(byte[] b1, byte[] b2) {
            return b1.Length == b2.Length && memcmp(b1, b2, b1.Length) == 0;
        }

        public static byte[] Max(byte[] b1, byte[] b2)
        {
            var minLength = Math.Min(b1.Length, b2.Length);
            if (minLength == 0)
            {
                return b1.Length > b2.Length ? b1 : b2;
            }

            var result = memcmp(b1, b2, minLength);
            if (result == 0)
            {
                if (b1.Length == b2.Length)
                {
                    return b1;
                }
                return b1.Length > b2.Length ? b1 : b2;
            }
            return result > 0 ? b1 : b2;
        }
    }
}