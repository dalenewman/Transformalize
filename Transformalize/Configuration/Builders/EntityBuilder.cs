namespace Transformalize.Configuration.Builders {

    public class EntityBuilder : IFieldHolder {

        private readonly ProcessBuilder _processBuilder;
        private readonly EntityConfigurationElement _entity;

        public EntityBuilder(ProcessBuilder processBuilder, EntityConfigurationElement entity) {
            _processBuilder = processBuilder;
            _entity = entity;
        }

        public ProcessConfigurationElement Process() {
            return _processBuilder.Process();
        }

        public EntityBuilder Version(string version) {
            _entity.Version = version;
            return this;
        }

        public EntityBuilder Alias(string alias) {
            _entity.Alias = alias;
            return this;
        }

        public EntityBuilder IndexOptimizations(bool optimize = true) {
            _entity.IndexOptimizations = optimize;
            return this;
        }

        public EntityBuilder Entity(string name) {
            return _processBuilder.Entity(name);
        }

        public FieldBuilder Field(string name) {
            var field = new FieldConfigurationElement() { Name = name };
            _entity.Fields.Add(field);
            return new FieldBuilder(this, field);
        }

        public RelationshipBuilder Relationship() {
            return _processBuilder.Relationship();
        }

        public EntityBuilder Prefix(string prefix) {
            _entity.Prefix = prefix;
            return this;
        }

        public FieldBuilder CalculatedField(string name) {
            var calculatedField = new FieldConfigurationElement() { Name = name };
            _entity.CalculatedFields.Add(calculatedField);
            return new FieldBuilder(this, calculatedField);
        }
    }
}