namespace Transformalize.Configuration.Builders {
    public class ActionBuilder {
        private readonly TemplateBuilder _templateBuilder;
        private readonly ActionConfigurationElement _action;

        public ActionBuilder(TemplateBuilder templateBuilder, ActionConfigurationElement action) {
            _templateBuilder = templateBuilder;
            _action = action;
        }

        public ActionBuilder File(string file) {
            _action.File = file;
            return this;
        }

        public ActionBuilder Connection(string name) {
            _action.Connection = name;
            return this;
        }

        public ActionBuilder Url(string url) {
            _action.Url = url;
            return this;
        }

        public TemplateBuilder Template(string name) {
            return _templateBuilder.Template(name);
        }

        public ActionBuilder Action(string action) {
            return _templateBuilder.Action(action);
        }

        public ActionBuilder Mode(string mode) {
            _action.Mode = mode;
            return this;
        }

        public EntityBuilder Entity(string name) {
            return _templateBuilder.Entity(name);
        }

        public SearchTypeBuilder SearchType(string name) {
            return _templateBuilder.SearchType(name);
        }

        public MapBuilder Map(string name) {
            return _templateBuilder.Map(name);
        }

        public ProcessBuilder TemplatePath(string path) {
            return _templateBuilder.TemplatePath(path);
        }

        public ProcessBuilder ScriptPath(string path) {
            return _templateBuilder.ScriptPath(path);
        }
    }
}