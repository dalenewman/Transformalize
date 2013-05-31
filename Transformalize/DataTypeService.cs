using System.Collections.Generic;
using Transformalize.Configuration;

namespace Transformalize {

    public static class DataTypeService {
        
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

        public static string GetSqlDbType(IField field) {
            var type = field.Type.ToLower().Replace("system.", string.Empty).ToLower();
            var length = type == "string" || type == "char"  || type == "byte[]" ? string.Concat("(", field.Length, ")") : string.Empty;
            var dimensions = type == "decimal" ? string.Format("({0},{1})", field.Precision, field.Scale) : string.Empty;
            return string.Concat(Types[type], length, dimensions);
        }
    }
}
