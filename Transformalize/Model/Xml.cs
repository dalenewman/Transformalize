using System.Collections.Generic;

namespace Transformalize.Model {

    public class Xml : BaseField, IField {

        private FieldSqlWriter _sqlWriter;
        public FieldSqlWriter SqlWriter {
            get { return _sqlWriter ?? (_sqlWriter = new FieldSqlWriter(this)); }
            set { _sqlWriter = value; }
        }

        public string XPath { get; set; }
        public int Index { get; set; }
        public Dictionary<string, Xml> InnerXml { get { return new Dictionary<string, Xml>(); } }
        
        public Xml(string typeName, bool output) : base(typeName, FieldType.Xml, output) {}

        public string AsJoin(string left, string right) {
            return "Never Join on XML! You Crazy???";
        }
       
    }
}