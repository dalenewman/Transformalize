using System.Globalization;

namespace Transformalize.Configuration.Builders {
    public class FieldBuilder {

        private readonly IFieldHolder _fieldHolder;
        private readonly FieldConfigurationElement _field;

        public FieldBuilder(IFieldHolder fieldHolder, FieldConfigurationElement field) {
            _fieldHolder = fieldHolder;
            _field = field;
        }

        public FieldBuilder Alias(string alias) {
            _field.Alias = alias;
            return this;
        }

        public FieldBuilder Type(string type) {
            _field.Type = type;
            return this;
        }

        public FieldBuilder QuotedWith(string quoteChar) {
            _field.QuotedWith = quoteChar;
            return this;
        }

        public FieldBuilder Optional(bool optional = true) {
            _field.Optional = optional;
            return this;
        }

        public FieldBuilder Default(string value) {
            _field.Default = value;
            return this;
        }

        public FieldBuilder Field(string name) {
            return _fieldHolder.Field(name);
        }

        public EntityBuilder Entity(string name) {
            return _fieldHolder.Entity(name);
        }

        public ProcessConfigurationElement Process() {
            return _fieldHolder.Process();
        }

        public FieldBuilder PrimaryKey(bool isPrimaryKey = true) {
            _field.PrimaryKey = isPrimaryKey;
            return this;
        }

        public RelationshipBuilder Relationship() {
            return _fieldHolder.Relationship();
        }

        public TransformBuilder Transform(string method = "") {
            var transform = new TransformConfigurationElement() { Method = method };
            _field.Transforms.Add(transform);
            return new TransformBuilder(this, transform);
        }

        public FieldBuilder Int16() {
            _field.Type = "System.Int16";
            return this;
        }

        public FieldBuilder Int() {
            _field.Type = "System.Int32";
            return this;
        }

        public FieldBuilder Int32() {
            _field.Type = "System.Int32";
            return this;
        }

        public FieldBuilder Int64() {
            _field.Type = "System.Int64";
            return this;
        }

        public FieldBuilder DateTime() {
            _field.Type = "System.DateTime";
            return this;
        }

        public FieldBuilder Single() {
            _field.Type = "System.Single";
            return this;
        }

        public FieldBuilder Decimal() {
            _field.Type = "System.Decimal";
            return this;
        }

        public FieldBuilder Decimal(int precision, int scale) {
            _field.Type = "System.Decimal";
            _field.Precision = precision;
            _field.Scale = scale;
            return this;
        }

        public FieldBuilder ByteArray() {
            _field.Type = "System.Byte[]";
            return this;
        }

        public FieldBuilder RowVersion() {
            _field.Type = "rowversion";
            _field.Length = "8";
            return this;
        }

        public FieldBuilder Length(string length) {
            _field.Length = length;
            return this;
        }

        public FieldBuilder Length(int length) {
            _field.Length = length.ToString(CultureInfo.InvariantCulture);
            return this;
        }

        public FieldBuilder Precision(int precision) {
            _field.Precision = precision;
            return this;
        }

        public FieldBuilder Scale(int scale) {
            _field.Scale = scale;
            return this;
        }

        public FieldBuilder CalculatedField(string name) {
            return _fieldHolder.CalculatedField(name);
        }

        public FieldBuilder Char() {
            _field.Type = "System.Char";
            _field.Length = "1";
            return this;
        }

        public FieldBuilder Input(bool input) {
            _field.Input = input;
            return this;
        }

        public FieldBuilder Output(bool output) {
            _field.Output = output;
            return this;
        }

        public FieldBuilder ReadInnerXml(bool readInnerXml) {
            _field.ReadInnerXml = readInnerXml;
            return this;
        }

        public FieldBuilder NodeType(string nodeType) {
            _field.NodeType = nodeType;
            return this;
        }

        public FieldBuilder Bool() {
            _field.Type = "System.Boolean";
            return this;
        }

        public FieldBuilder Boolean() {
            _field.Type = "System.Boolean";
            return this;
        }

        public FieldBuilder Aggregate(string method) {
            _field.Aggregate = method;
            return this;
        }

    }

}