namespace Transformalize.Configuration.Builders {
    public class IoBuilder {
        private readonly EntityBuilder _entityBuilder;
        private readonly IoConfigurationElement _output;

        public IoBuilder(EntityBuilder entityBuilder, IoConfigurationElement output) {
            _entityBuilder = entityBuilder;
            _output = output;
        }

        public FieldBuilder Field(string name) {
            return _entityBuilder.Field(name);
        }

        public IoBuilder Output(string name, string connectionName) {
            return _entityBuilder.Output(name, connectionName);
        }

        public IoBuilder RunField(string alias) {
            _output.RunField = alias;
            return this;
        }

        public IoBuilder RunOperator(string op) {
            _output.RunOperator = op;
            return this;
        }

        public IoBuilder RunType(string type) {
            _output.RunType = type;
            return this;
        }

        public IoBuilder RunValue(object value) {
            _output.RunValue = value.ToString();
            return this;
        }

        public IoBuilder Input(string name, string connectionName) {
            return _entityBuilder.Input(name, connectionName);
        }
    }
}