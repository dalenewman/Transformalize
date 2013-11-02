namespace Transformalize.Configuration.Builders {
    public class ParameterBuilder {
        private readonly TransformBuilder _transformBuilder;
        private readonly ParameterConfigurationElement _parameter;

        public ParameterBuilder(TransformBuilder transformBuilder, ParameterConfigurationElement parameter) {
            _transformBuilder = transformBuilder;
            _parameter = parameter;
        }

        public ParameterBuilder Field(string field) {
            _parameter.Field = field;
            return this;
        }

        public ParameterBuilder Field(string entity, string field) {
            _parameter.Entity = entity;
            _parameter.Field = field;
            return this;
        }

        public ParameterBuilder Type(string type) {
            _parameter.Type = type;
            return this;
        }

        public EntityBuilder Entity(string name) {
            return _transformBuilder.Entity(name);
        }

        public ParameterBuilder Name(string name) {
            _parameter.Name = name;
            return this;
        }

        public TransformBuilder Transform(string method) {
            return _transformBuilder.Transform(method);
        }

        public TransformBuilder Transform() {
            return _transformBuilder.Transform();
        }

        public FieldBuilder CalculatedField(string name) {
            return _transformBuilder.CalculatedField(name);
        }

        public ParameterBuilder Parameter(string field) {
            return _transformBuilder.Parameter(field);
        }

        public ParameterBuilder Parameter() {
            return _transformBuilder.Parameter();
        }

        public ProcessConfigurationElement Process() {
            return _transformBuilder.Process();
        }
    }
}