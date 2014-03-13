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
    }
}