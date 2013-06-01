using System;
using System.Collections.Generic;

namespace Transformalize.Model {
    public class Field : IField {

        private string _sqlDataType;
        private Type _realType;
        private string _simpleType;

        public Dictionary<string, Xml> Xml = new Dictionary<string, Xml>();
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

        public string SqlDataType() {
            return _sqlDataType ?? (_sqlDataType = DataTypeService.GetSqlDbType(this));
        }

        public string SimpleType() {
            return _simpleType ?? (_simpleType = Type.ToLower().Replace("system.", string.Empty));
        }

        public Type RealType() {
            return _realType ?? (_realType = System.Type.GetType(this.Type));
        }
    }
}