/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using Transformalize.Model;

namespace Transformalize {

    public class SqlServerDataTypeService : IDataTypeService {

        private static readonly Dictionary<string, string> Types = new Dictionary<string, string> {
            {"int64", "BIGINT"},
            {"boolean", "BIT"},
            {"string", "NVARCHAR"},
            {"datetime", "DATETIME"},
            {"decimal", "DECIMAL"},
            {"double", "FLOAT"},
            {"int32", "INT"},
            {"char", "NCHAR"},
            {"single", "REAL"},
            {"int16", "SMALLINT"},
            {"byte", "TINYINT"},
            {"byte[]", "BINARY"},
            {"guid", "UNIQUEIDENTIFIER"},
            {"rowversion", "ROWVERSION"}
        };

        public string GetDataType(Field field) {

            var length = field.SimpleType == "string" || field.SimpleType == "char" || field.SimpleType == "byte[]" ? string.Concat("(", field.Length, ")") : string.Empty;
            var dimensions = field.SimpleType == "decimal" ? string.Format("({0},{1})", field.Precision, field.Scale) : string.Empty;
            var notNull = field.NotNull ? " NOT NULL" : string.Empty;
            var surrogate = field.Clustered ? " IDENTITY(1,1) UNIQUE CLUSTERED" : string.Empty;
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

    public interface IDataTypeService {
        string GetDataType(Field field);
    }
}
