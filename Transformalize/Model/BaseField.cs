using System.Linq;

namespace Transformalize.Model {
    public abstract class BaseField {

        private string _type;
        public string Type {
            get { return _type; }
            set {
                _simpleType = null;
                _quote = null;
                _type = value;
            }
        }

        private string _simpleType;
        public string SimpleType {
            get { return _simpleType ?? (_simpleType = Type.ToLower().Replace("system.", string.Empty)); }
        }

        private string _quote;
        public string Quote {
            get { return _quote ?? (_quote = (new[] { "string", "char", "datetime", "guid" }).Any(t => t.Equals(SimpleType)) ? "'" : string.Empty); }
        }

        private string _sqlDataType;
        private string _alias;

        public string SqlDataType {
            get { return _sqlDataType ?? (_sqlDataType = DataTypeService.GetSqlDbType(this)); }
        }

        public string Alias {
            get {
                return _alias ?? Name;
            }
            set { _alias = value; }
        }

        public string Schema { get; set; }
        public string Entity { get; set; }
        public string Parent { get; set; }
        public string Name { get; set; }

        public int Length { get; set; }
        public int Precision { get; set; }
        public int Scale { get; set; }
        public object Default { get; set; }
        public bool Output { get; set; }
        public FieldType FieldType { get; set; }

    }
}