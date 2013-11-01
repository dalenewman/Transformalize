using System.Runtime.InteropServices;

namespace Transformalize.Configuration.Builders {
    public class FieldBuilder {
        private readonly ProcessBuilder _processBuilder;
        private readonly EntityBuilder _entityBuilder;
        private readonly FieldConfigurationElement _field;

        public FieldBuilder(ProcessBuilder processBuilder, EntityBuilder entityBuilder, FieldConfigurationElement field) {
            _processBuilder = processBuilder;
            _entityBuilder = entityBuilder;
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

        public FieldBuilder Default(string value) {
            _field.Default = value;
            return this;
        }

        public FieldBuilder Field(string name) {
            return _entityBuilder.Field(name);
        }

        public EntityBuilder Entity(string name) {
            return _processBuilder.Entity(name);
        }

        public ProcessConfigurationElement Process() {
            return _processBuilder.Process();
        }

        public FieldBuilder PrimaryKey() {
            _field.PrimaryKey = true;
            return this;
        }

        public RelationshipBuilder Relationship() {
            return _processBuilder.Relationship();
        }

        public TransformBuilder Transform() {
            var transform = new TransformConfigurationElement();
            _field.Transforms.Add(transform);
            return new TransformBuilder(this, transform);
        }
    }
}