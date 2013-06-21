using System.Collections.Generic;
using System.Linq;

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

        public Field() {
            InnerXml = new Dictionary<string, Xml>();
            FieldType = FieldType.Field;
        }
        
        public string AsJoin(string left, string right) {
            return string.Format("{0}.[{1}] = {2}.[{1}]", left, Name, right);
        }

    }
}