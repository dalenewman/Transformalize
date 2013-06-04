using System;
using System.Collections.Generic;
using System.Linq;

namespace Transformalize.Model {
    public class Field : IField {

        private string _sqlDataType;
        private Type _realType;
        private string _simpleType;

        public string Schema { get; set; }
        public string Entity { get; set; }
        public string Parent { get { return string.Empty; } }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Alias { get; set; }
        public int Length { get; set; }
        public int Precision { get; set; }
        public int Scale { get; set; }
        public bool Output { get; set; }
        public FieldType FieldType { get; set; }

        public Field() {
            InnerXml = new Dictionary<string, Xml>();
        }

        public string SqlDataType() {
            return _sqlDataType ?? (_sqlDataType = DataTypeService.GetSqlDbType(this));
        }

        public string AsSelect() {
            return Alias.Equals(Name) ?
                string.Format("t.[{0}]", Name) :
                string.Format("[{0}] = t.[{1}]", Alias, Name);
        }

        public string SimpleType() {
            return _simpleType ?? (_simpleType = Type.ToLower().Replace("system.", string.Empty));
        }

        public Type RealType() {
            return _realType ?? (_realType = System.Type.GetType(this.Type));
        }

        public bool NeedsQuotes() {
            return (new[] { "string", "char", "datetime", "guid" }).Any(t => t.Equals(SimpleType()));
        }

        public string AsJoin(string left, string right) {
            return string.Format("{0}.[{1}] = {2}.[{1}]", left, Name, right);
        }

        public Dictionary<string, Xml> InnerXml { get; set; }
        public string AsDefinition() {
            return string.Format("[{0}] {1} NOT NULL", Alias, SqlDataType());
        }
    }
}