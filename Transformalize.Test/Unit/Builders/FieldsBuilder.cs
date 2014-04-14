namespace Transformalize.Test.Unit.Builders {

    public class FieldsBuilder {

        private readonly Main.Fields _fields = new Main.Fields();

        public FieldBuilder Field(string name) {
            var field = new Main.Field(Main.FieldType.Field) { Name = name, Alias = name };
            return Consume(field);
        }

        private FieldBuilder Consume(Main.Field field) {
            _fields.Add(field);
            var fieldsBuilder = this;
            return new FieldBuilder(ref fieldsBuilder, ref field);
        }

        public Main.Fields ToFields() {
            return _fields;
        }
    }

    public class FieldBuilder {

        private readonly FieldsBuilder _fieldsBuilder;
        private readonly Main.Field _field;

        public FieldBuilder(ref FieldsBuilder fieldsBuilder, ref Main.Field field) {
            _fieldsBuilder = fieldsBuilder;
            _field = field;
        }

        public FieldBuilder Type(string type) {
            _field.Type = type;
            return this;
        }

        public FieldBuilder Length(string length) {
            _field.Length = length;
            return this;
        }

        public FieldBuilder NodeType(string nodeType) {
            _field.NodeType = nodeType;
            return this;
        }

        public FieldBuilder ReadInnerXml(bool option) {
            _field.ReadInnerXml = option;
            return this;
        }

        public FieldBuilder Field(string name) {
            return _fieldsBuilder.Field(name);
        }

        public Main.Fields ToFields() {
            return _fieldsBuilder.ToFields();
        }
    }
}
