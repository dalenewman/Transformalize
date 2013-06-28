using System.Collections.Generic;
using Transformalize.Model;

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

        public static string GetSqlDbType(BaseField field) {
            var length = field.SimpleType == "string" || field.SimpleType == "char" || field.SimpleType == "byte[]" ? string.Concat("(", field.Length, ")") : string.Empty;
            var dimensions = field.SimpleType == "decimal" ? string.Format("({0},{1})", field.Precision, field.Scale) : string.Empty;
            var notNull = field.NotNull ? " NOT NULL" : string.Empty;
            var surrogate = field.Clustered ? " IDENTITY(1,1) UNIQUE CLUSTERED" : string.Empty;
            return string.Concat(Types[field.SimpleType], length, dimensions, notNull, surrogate);
        }
    }
}
