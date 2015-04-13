using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Main;

namespace Transformalize.Configuration.Builders {

    public class ProcessBuilder : IFieldHolder, IActionHolder {

        private readonly TflProcess _process;

        public ProcessBuilder(string name) {
            _process = new TflProcess() { Name = name };
        }

        public ProcessBuilder(TflProcess element) {
            _process = element;
        }

        public TflProcess Process() {
            return new TflRoot(_process).Processes[0];
        }

        public ConnectionBuilder Connection(string name) {
            var connection = _process.GetDefaultOf<TflConnection>();
            connection.Name = name;
            _process.Connections.Add(connection);
            return new ConnectionBuilder(this, connection);
        }

        public ConnectionBuilder Connection(TflConnection connection) {
            _process.Connections.Add(connection);
            return new ConnectionBuilder(this, connection);
        }

        public MapBuilder Map(string name) {
            var map = _process.GetDefaultOf<TflMap>();
            map.Name = name;
            _process.Maps.Add(map);
            return new MapBuilder(this, map);
        }

        public MapBuilder Map(string name, string sql) {
            var map = _process.GetDefaultOf<TflMap>();
            map.Name = name;
            map.Query = sql;
            _process.Maps.Add(map);
            return new MapBuilder(this, map);
        }

        public EntityBuilder Entity(string name) {
            var entity = _process.GetDefaultOf<TflEntity>(e => e.Name = name);
            _process.Entities.Add(entity);
            return new EntityBuilder(this, entity);
        }

        public RelationshipBuilder Relationship() {
            var relationship = _process.GetDefaultOf<TflRelationship>();
            _process.Relationships.Add(relationship);
            return new RelationshipBuilder(this, relationship);
        }

        public TemplateBuilder Template(string name) {
            var template = _process.GetDefaultOf<TflTemplate>();
            template.Name = name;
            _process.Templates.Add(template);
            return new TemplateBuilder(this, template);
        }


        public ActionBuilder Action(string action) {
            var a = _process.GetDefaultOf<TflAction>();
            a.Action = action;
            _process.Actions.Add(a);
            return new ActionBuilder(this, a);
        }

        public SearchTypeBuilder SearchType(string name) {
            var searchType = _process.GetDefaultOf<TflSearchType>();
            searchType.Name = name;
            _process.SearchTypes.Add(searchType);
            return new SearchTypeBuilder(this, searchType);
        }

        public FieldBuilder CalculatedField(string name) {
            var cf = _process.GetDefaultOf<TflField>(f => f.Name = name);
            _process.CalculatedFields.Add(cf);
            return new FieldBuilder(this, cf);
        }

        public FieldBuilder Field(string name) {
            var calculatedField = _process.GetDefaultOf<TflField>(f => f.Name = name);
            _process.CalculatedFields.Add(calculatedField);
            return new FieldBuilder(this, calculatedField);
        }

        public ProcessBuilder TemplatePath(string path) {
            foreach (var template in _process.Templates) {
                template.Path = path;
            }
            return this;
        }

        public ProcessBuilder ScriptPath(string path) {
            foreach (var script in _process.Scripts) {
                script.Path = path;
            }
            return this;
        }

        public ProcessBuilder PipelineThreading(PipelineThreading pipelineThreading) {
            _process.PipelineThreading = pipelineThreading.ToString();
            return this;
        }

        public ScriptBuilder Script(string name) {
            var script = _process.GetDefaultOf<TflScript>();
            script.Name = name;
            _process.Scripts.Add(script);
            return new ScriptBuilder(this, script);
        }

        public ProcessBuilder Script(string name, string fileName) {
            var script = _process.GetDefaultOf<TflScript>();
            script.Name = name;
            script.File = fileName;
            _process.Scripts.Add(script);
            return this;
        }

        public ProcessBuilder Star(string star) {
            _process.Star = star;
            return this;
        }

        public ProcessBuilder Mode(string mode) {
            _process.Mode = mode;
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