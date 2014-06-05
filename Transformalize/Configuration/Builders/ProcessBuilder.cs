using Transformalize.Main;

namespace Transformalize.Configuration.Builders {

    public class ProcessBuilder : IFieldHolder, IActionHolder {

        private readonly ProcessConfigurationElement _process;

        public ProcessBuilder(string name) {
            _process = new ProcessConfigurationElement() { Name = name };
            _process.SearchTypes.Add(
                new SearchTypeConfigurationElement {
                    Name = "none",
                    MultiValued = false,
                    Store = false,
                    Index = false
                },
                new SearchTypeConfigurationElement {
                    Name = "default",
                    MultiValued = false,
                    Store = true,
                    Index = true
                }
            );

        }

        public ProcessConfigurationElement Process() {
            return _process;
        }

        public ConnectionBuilder Connection(string name) {
            var connection = new ConnectionConfigurationElement() { Name = name };
            _process.Connections.Add(connection);
            return new ConnectionBuilder(this, connection);
        }

        public ConnectionBuilder Connection(ConnectionConfigurationElement connection) {
            _process.Connections.Add(connection);
            return new ConnectionBuilder(this, connection);
        }

        public MapBuilder Map(string name) {
            var map = new MapConfigurationElement { Name = name };
            _process.Maps.Add(map);
            return new MapBuilder(this, map);
        }

        public MapBuilder Map(string name, string sql) {
            var map = new MapConfigurationElement { Name = name };
            map.Items.Sql = sql;
            _process.Maps.Add(map);
            return new MapBuilder(this, map);
        }

        public EntityBuilder Entity(string name) {
            var entity = new EntityConfigurationElement() { Name = name };
            _process.Entities.Add(entity);
            return new EntityBuilder(this, entity);
        }

        public RelationshipBuilder Relationship() {
            var relationship = new RelationshipConfigurationElement();
            _process.Relationships.Add(relationship);
            return new RelationshipBuilder(this, relationship);
        }

        public TemplateBuilder Template(string name) {
            var template = new TemplateConfigurationElement { Name = name };
            _process.Templates.Add(template);
            return new TemplateBuilder(this, template);
        }


        public ActionBuilder Action(string action) {
            var a = new ActionConfigurationElement() { Action = action };
            _process.Actions.Add(a);
            return new ActionBuilder(this, a);
        }

        public SearchTypeBuilder SearchType(string name) {
            var searchType = new SearchTypeConfigurationElement() { Name = name };
            _process.SearchTypes.Add(searchType);
            return new SearchTypeBuilder(this, searchType);
        }

        public FieldBuilder CalculatedField(string name) {
            var calculatedField = new FieldConfigurationElement() { Name = name };
            _process.CalculatedFields.Add(calculatedField);
            return new FieldBuilder(this, calculatedField);
        }

        public FieldBuilder Field(string name) {
            var calculatedField = new FieldConfigurationElement() { Name = name };
            _process.CalculatedFields.Add(calculatedField);
            return new FieldBuilder(this, calculatedField);
        }

        public ProcessBuilder TemplatePath(string path) {
            _process.Templates.Path = path;
            return this;
        }

        public ProcessBuilder ScriptPath(string path) {
            _process.Scripts.Path = path;
            return this;
        }

        public ProcessBuilder PipelineThreading(PipelineThreading pipelineThreading) {
            _process.PipelineThreading = pipelineThreading.ToString();
            return this;
        }

        public ScriptBuilder Script(string name) {
            var script = new ScriptConfigurationElement() { Name = name };
            _process.Scripts.Add(script);
            return new ScriptBuilder(this, script);
        }

        public ProcessBuilder Script(string name, string fileName) {
            var script = new ScriptConfigurationElement() { Name = name, File = fileName };
            _process.Scripts.Add(script);
            return this;
        }

        public ProcessBuilder Star(string star) {
            _process.Star = star;
            return this;
        }

        public ProcessBuilder StarEnabled(bool enabled = true) {
            _process.StarEnabled = enabled;
            return this;
        }

        public ProcessBuilder TimeZone(string timeZone) {
            _process.TimeZone = timeZone;
            return this;
        }

    }
}