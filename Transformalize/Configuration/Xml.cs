namespace Transformalize.Configuration {

    public class Xml : IField {
        private string _sqlDataType;

        public string XPath;
        public int Index;
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
        public FieldType FieldType { get { return FieldType.Xml;}}
        
        public string SqlDataType() {
            return _sqlDataType ?? (_sqlDataType = DataTypeService.GetSqlDbType(this));
        }
    }
}