using System.Collections.Generic;

namespace Transformalize.Model {

    public class Xml : IField {
        private string _sqlDataType;

        public string XPath { get; set; }
        public int Index { get; set; }
        public string Schema { get; set; }
        public string Entity { get; set; }
        public string Parent { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Alias { get; set; }
        public int Length { get; set; }
        public int Precision { get; set; }
        public int Scale { get; set; }
        public bool Output { get; set; }
        public FieldType FieldType { get { return FieldType.Xml; } }

        public string SqlDataType() {
            return _sqlDataType ?? (_sqlDataType = DataTypeService.GetSqlDbType(this));
        }

        public string AsSelect() {
            return string.Format("[{0}] = t.[{1}].value('({2})[1]','{3}')", Alias, Parent, XPath, SqlDataType());
        }

        public string AsJoin(string left, string right) {
            return "Never Join on XML! You Crazy???";
        }

        public Dictionary<string, Xml> InnerXml { get { return new Dictionary<string, Xml>();} }

        public string AsDefinition() {
            return string.Format("[{0}] {1} NOT NULL", Alias, SqlDataType());
        }
    }
}