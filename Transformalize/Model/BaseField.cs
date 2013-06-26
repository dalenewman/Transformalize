using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transformalize.Transforms;

namespace Transformalize.Model {

    public abstract class BaseField {

        private string _type;
        private string _simpleType;
        private string _quote;
        private string _sqlDataType;
        private string _alias;
        private bool _output;

        public string Schema { get; set; }
        public string Entity { get; set; }
        public string Parent { get; set; }
        public string Name { get; set; }
        public bool Input { get; set; }
        public int Length { get; set; }
        public int Precision { get; set; }
        public int Scale { get; set; }
        public object Default { get; set; }
        public bool Clustered { get; set; }
        public FieldType FieldType { get; set; }
        public KeyValuePair<string, string> References { get; set; }
        public StringBuilder StringBuilder { get; set; }
        public ITransform[] Transforms { get; set; }

        public string Type {
            get { return _type; }
            set {
                _simpleType = null;
                _quote = null;
                _type = value;
            }
        }

        public string SimpleType {
            get { return _simpleType ?? (_simpleType = Type.ToLower().Replace("system.", string.Empty)); }
        }

        public string Quote {
            get { return _quote ?? (_quote = (new[] { "string", "char", "datetime", "guid" }).Any(t => t.Equals(SimpleType)) ? "'" : string.Empty); }
        }

        public string SqlDataType {
            get { return _sqlDataType ?? (_sqlDataType = DataTypeService.GetSqlDbType(this)); }
        }

        public string Alias {
            get {
                return _alias ?? Name;
            }
            set { _alias = value; }
        }

        /// <summary>
        /// Output is set in field configuration, but also is always true if FieldType is any type of key (for updating purposes)
        /// </summary>
        public bool Output {
            get {
                return FieldType == FieldType.MasterKey || FieldType == FieldType.ForeignKey || FieldType == FieldType.PrimaryKey || _output;
            }
            set { _output = value; }
        }

    }
}