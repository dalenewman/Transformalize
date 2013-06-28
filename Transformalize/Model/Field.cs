using System.Collections.Generic;

namespace Transformalize.Model {
    public class Field : BaseField, IField {

        private FieldSqlWriter _sqlWriter;
        public FieldSqlWriter SqlWriter {
            get { return _sqlWriter ?? (_sqlWriter = new FieldSqlWriter(this)); }
            set { _sqlWriter = value; }
        }

        public string XPath { get { return string.Empty; } }
        public int Index { get { return 1; } }
        public Dictionary<string, Xml> InnerXml { get; set; }
        
        public Field(string typeName, int length, FieldType fieldType, bool output)
            : base(typeName, length, fieldType, output) {
            InnerXml = new Dictionary<string, Xml>();
        }

        // defaults to string field output
        public Field() : base("System.String", 64, FieldType.Field, true) {
            InnerXml = new Dictionary<string, Xml>();
        }

        public string AsJoin(string left, string right) {
            return string.Format("{0}.[{1}] = {2}.[{1}]", left, Name, right);
        }

    }
}