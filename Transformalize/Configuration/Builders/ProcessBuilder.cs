using System.Collections.Generic;
using System.Linq;
using Cfg.Net.Ext;
using Transformalize.Main;

namespace Transformalize.Configuration.Builders {

    public class ProcessBuilder : IFieldHolder, IActionHolder {

        private readonly TflProcess _process;

        public ProcessBuilder(string name) {
            _process = new TflProcess { Name = name }.WithDefaults();
        }

        public ProcessBuilder(TflProcess element) {
            _process = element;
        }

        public TflProcess Process() {
            return new TflRoot(_process).Processes[0];
        }

        public ConnectionBuilder Connection(string name) {
            if (_process.Connections.Any(c => c.Name == name)) {
                return new ConnectionBuilder(this, _process.Connections.First(c => c.Name == name));
            }
            var connection = new TflConnection { Name = name }.WithDefaults();
            _process.Connections.Add(connection);
            return new ConnectionBuilder(this, connection);
        }

        public ConnectionBuilder Connection(TflConnection connection) {
            var name = connection.Name;
            if (_process.Connections.Any(c => c.Name == name)) {
                var cn = _process.Connections.First(c => c.Name == name);
                _process.Connections.Remove(cn);
                _process.Connections.Add(connection);
                return new ConnectionBuilder(this, connection);
            }

            _process.Connections.Add(connection);
            return new ConnectionBuilder(this, connection);
        }

        public MapBuilder Map(string name) {
            var map = new TflMap { Name = name }.WithDefaults();
            _process.Maps.Add(map);
            return new MapBuilder(this, map);
        }

        public MapBuilder Map(string name, string sql) {
            var map = new TflMap { Name = name, Query = sql }.WithDefaults();
            _process.Maps.Add(map);
            return new MapBuilder(this, map);
        }

        public EntityBuilder Entity(string name) {
            var entity = new TflEntity { Name = name }.WithDefaults();
            _process.Entities.Add(entity);
            return new EntityBuilder(this, entity);
        }

        public RelationshipBuilder Relationship() {
            var relationship = new TflRelationship().WithDefaults();
            _process.Relationships.Add(relationship);
            return new RelationshipBuilder(this, relationship);
        }

        public TemplateBuilder Template(string name) {
            var template = new TflTemplate().WithDefaults();
            template.Name = name;
            _process.Templates.Add(template);
            return new TemplateBuilder(this, template);
        }


        public ActionBuilder Action(string action) {
            var a = new TflAction { Action = action }.WithDefaults();
            _process.Actions.Add(a);
            return new ActionBuilder(this, a);
        }

        public SearchTypeBuilder SearchType(string name) {
            var searchType = new TflSearchType().WithDefaults();
            searchType.Name = name;
            _process.SearchTypes.Add(searchType);
            return new SearchTypeBuilder(this, searchType);
        }

        public FieldBuilder CalculatedField(string name) {
            var cf = new TflField { Name = name }.WithDefaults();
            _process.CalculatedFields.Add(cf);
            return new FieldBuilder(this, cf);
        }

        public FieldBuilder Field(string name) {
            var calculatedField = new TflField { Name = name }.WithDefaults();
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
            var script = new TflScript().WithDefaults();
            script.Name = name;
            _process.Scripts.Add(script);
            return new ScriptBuilder(this, script);
        }

        public ProcessBuilder Script(string name, string fileName) {
            var script = new TflScript {
                Name = name,
                File = fileName
            }.WithDefaults();
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

        public ProcessBuilder DataSet(string name, IEnumerable<Dictionary<string, string>> rows) {
            var dataSet = new TflDataSet { Name = name, Rows = rows.ToList() };
            _process.DataSets.Add(dataSet);
            return this;
        }
    }
}