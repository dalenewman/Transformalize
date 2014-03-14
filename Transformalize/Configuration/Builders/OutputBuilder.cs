namespace Transformalize.Configuration.Builders {
    public class OutputBuilder {
        private readonly EntityBuilder _entityBuilder;
        private readonly OutputConfigurationElement _output;

        public OutputBuilder(EntityBuilder entityBuilder, OutputConfigurationElement output) {
            _entityBuilder = entityBuilder;
            _output = output;
        }

        public OutputBuilder Connection(string name) {
            _output.Connection = name;
            return this;
        }

        public FieldBuilder Field(string name) {
            return _entityBuilder.Field(name);
        }

        public OutputBuilder Output(string name) {
            return _entityBuilder.Output(name);
        }

        public OutputBuilder RunField(string alias) {
            _output.RunField = alias;
            return this;
        }

        public OutputBuilder RunOperator(string op) {
            _output.RunOperator = op;
            return this;
        }

        public OutputBuilder RunType(string type) {
            _output.RunType = type;
            return this;
        }

        public OutputBuilder RunValue(object value) {
            _output.RunValue = value.ToString();
            return this;
        }
    }
}