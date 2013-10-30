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

using System.Collections.Generic;
using System.Linq;

namespace Transformalize.Main.Providers.SqlServer {
    public class SqlServerDataTypeService : IDataTypeService {
        private Dictionary<string, string> _reverseTypes;
        private Dictionary<string, string> _types;

        public Dictionary<string, string> Types {
            get {
                if (_types == null) {
                    _types = new Dictionary<string, string> {
                        {"int64", "BIGINT"},
                        {"int", "INT"},
                        {"long", "BIGINT"},
                        {"boolean", "BIT"},
                        {"bool", "BIT"},
                        {"string", "NVARCHAR"},
                        {"datetime", "DATETIME"},
                        {"decimal", "DECIMAL"},
                        {"double", "FLOAT"},
                        {"int32", "INT"},
                        {"char", "NCHAR"},
                        {"single", "REAL"},
                        {"int16", "SMALLINT"},
                        {"byte", "TINYINT"},
                        {"byte[]", "VARBINARY"},
                        {"guid", "UNIQUEIDENTIFIER"},
                        {"rowversion", "BINARY"},
                        {"xml", "XML"}
                    };
                }
                return _types;
            }
        }

        public Dictionary<string, string> TypesReverse {
            get {
                if (_reverseTypes == null) {
                    _reverseTypes = new Dictionary<string, string> {
                        {"BIGINT", "System.Int64"},
                        {"BIT", "System.Boolean"},
                        {"NVARCHAR", "System.String"},
                        {"DATE", "System.DateTime"},
                        {"DATETIME", "System.DateTime"},
                        {"SMALLDATETIME", "System.DateTime"},
                        {"DECIMAL", "System.Decimal"},
                        {"NUMERIC", "System.Decimal"},
                        {"MONEY", "System.Decimal"},
                        {"FLOAT", "System.Decimal"},
                        {"INT", "System.Int32"},
                        {"CHAR", "System.Char"},
                        {"NCHAR", "System.Char"},
                        {"REAL", "System.Single"},
                        {"SMALLINT", "System.Int16"},
                        {"TINYINT", "System.Byte"},
                        {"UNIQUEIDENTIFIER", "System.Guid"},
                        {"ROWVERSION", "System.Byte[]"},
                        {"TIMESTAMP", "System.Byte[]"},
                        {"IMAGE", "System.Byte[]"},
                        {"BINARY", "System.Byte[]"},
                        {"VARBINARY", "System.Byte[]"},
                        {"NTEXT", "System.String"},
                        {"XML", "System.Xml"}
                    };
                }
                return _reverseTypes;
            }
        }

        public string GetDataType(Field field) {
            var length = (new[] { "string", "char", "binary", "byte[]", "rowversion", "varbinary" }).Any(t => t == field.SimpleType) ? string.Concat("(", field.Length, ")") : string.Empty;
            var dimensions = (new[] { "decimal", "double" }).Any(s => s.Equals(field.SimpleType)) ? string.Format("({0},{1})", field.Precision, field.Scale) : string.Empty;
            var notNull = field.NotNull ? " NOT NULL" : string.Empty;
            var surrogate = field.Identity ? " IDENTITY(1,1) " : string.Empty;
            var sqlDataType = Types[field.SimpleType];

            if (!field.Unicode && sqlDataType.StartsWith("N")) {
                sqlDataType = sqlDataType.TrimStart("N".ToCharArray());
            }

            if (!field.VariableLength && sqlDataType.EndsWith("VARCHAR")) {
                sqlDataType = sqlDataType.Replace("VAR", string.Empty);
            }

            return string.Concat(sqlDataType, length, dimensions, notNull, surrogate);
        }
    }
}