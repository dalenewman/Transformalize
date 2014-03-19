using System.Collections.Generic;
using System.Linq;

namespace Transformalize.Main.Providers.SqlServer
{
    public class MySqlDataTypeService : IDataTypeService {
        // note: just a copy of the PostgreSql one for now
        private Dictionary<string, string> _reverseTypes;
        private Dictionary<string, string> _types;

        public Dictionary<string, string> Types {
            get {
                if (_types == null) {
                    _types = new Dictionary<string, string> {
                        {"int64", "BIGINT"},
                        {"int", "INTEGER"},
                        {"long", "BIGINT"},
                        {"boolean", "BIT"},
                        {"bool", "BIT"},
                        {"string", "VARCHAR"},
                        {"datetime", "DATETIME"},
                        {"decimal", "NUMERIC"},
                        {"double", "DOUBLE"},
                        {"int32", "INTEGER"},
                        {"char", "VARCHAR"},
                        {"single", "REAL"},
                        {"int16", "SMALLINT"},
                        {"byte", "SMALLINT"},
                        {"byte[]", "BYTEA"},
                        {"guid", "UUID"},
                        {"rowversion", "BYTEA"},
                        {"xml", "TEXT"}
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
                        {"VARCHAR", "System.String"},
                        {"DATE", "System.DateTime"},
                        {"DATETIME", "System.DateTime"},
                        {"TIME", "System.DateTime"},
                        {"NUMERIC", "System.Decimal"},
                        {"MONEY", "System.Decimal"},
                        {"REAL", "System.Single"},
                        {"INTEGER", "System.Int32"},
                        {"SMALLINT", "System.Int16"},
                        {"UUID", "System.Guid"},
                        {"TIMESTAMP", "System.DateTime"},
                        {"BYTEA", "System.Byte[]"},
                        {"TEXT", "System.String"}
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

            return string.Concat(sqlDataType, length, dimensions, notNull, surrogate);
        }
    }
}