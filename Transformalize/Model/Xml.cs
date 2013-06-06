using System.Collections.Generic;
using System.Linq;

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
        
        public string AsJoin(string left, string right) {
            return "Never Join on XML! You Crazy???";
        }
       
    }
}