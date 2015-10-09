using Cfg.Net;
using Cfg.Net.Ext;
using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Configuration.Builders {

    public class TemplateBuilder : IActionHolder {

        private readonly ProcessBuilder _processBuilder;
        private readonly TflTemplate _template;

        public TemplateBuilder(ProcessBuilder processBuilder, TflTemplate template) {
            _processBuilder = processBuilder;
            _template = template;
        }

        public TemplateBuilder File(string name) {
            _template.File = name;
            return this;
        }

        public TemplateBuilder Cache(bool cache) {
            _template.Cache = cache;
            return this;
        }

        public TemplateBuilder Enabled(bool enabled) {
            _template.Enabled = enabled;
            return this;
        }

        public TemplateBuilder ContentType(string contentType) {
            _template.ContentType = contentType;
            return this;
        }

        public TemplateBuilder Template(string name) {
            return _processBuilder.Template(name);
        }

        public EntityBuilder Entity(string name) {
            return _processBuilder.Entity(name);
        }

        public ActionBuilder Action(string action) {
            var a = _template.GetDefaultOf<TflAction>(x => x.Action = action);
            _template.Actions.Add(a);
            return new ActionBuilder(this, a);
        }

        public SearchTypeBuilder SearchType(string name) {
            return _processBuilder.SearchType(name);
        }

        public MapBuilder Map(string name) {
            return _processBuilder.Map(name);
        }

        public ProcessBuilder TemplatePath(string path) {
            return _processBuilder.TemplatePath(path);
        }

        public ProcessBuilder ScriptPath(string path) {
            return _processBuilder.ScriptPath(path);
        }

        public TflProcess Process() {
            return _processBuilder.Process();
        }

        public TemplateBuilder Parameter(string name, object value) {
            var parameter = _template.GetDefaultOf<TflParameter>(p => {
                p.Name = name;
                p.Value = value.ToString();
            });
            _template.Parameters.Add(parameter);
            return this;
        }

        public TemplateBuilder Parameter(string name, object value, string type) {
            var parameter = _template.GetDefaultOf<TflParameter>(p => {
                p.Name = name;
                p.Value = value.ToString();
                p.Type = type;
            }); _template.Parameters.Add(parameter);
            return this;
        }

    }
}