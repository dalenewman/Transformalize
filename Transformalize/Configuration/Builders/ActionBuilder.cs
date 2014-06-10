namespace Transformalize.Configuration.Builders {
    public class ActionBuilder {
        private readonly IActionHolder _builder;
        private readonly ActionConfigurationElement _action;

        public ActionBuilder(IActionHolder builder, ActionConfigurationElement action) {
            _builder = builder;
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
            return _builder.Template(name);
        }

        public ActionBuilder Action(string action) {
            return _builder.Action(action);
        }

        public ActionBuilder Mode(string mode) {
            _action.Mode = mode;
            return this;
        }

        public ActionBuilder Modes(params string[] modes) {
            foreach (var mode in modes) {
                _action.Modes.Add(new ModeConfigurationElement() { Mode = mode });
            }
            return this;
        }

        public ActionBuilder Cc(string cc) {
            _action.Cc = cc;
            return this;
        }

        public ActionBuilder Body(string body) {
            _action.Body = body;
            return this;
        }

        public ActionBuilder Before(bool runBefore) {
            _action.Before = runBefore;
            return this;
        }

        public ActionBuilder After(bool runAfter) {
            _action.After = runAfter;
            return this;
        }

        public ActionBuilder From(string from) {
            _action.From = from;
            return this;
        }

        public ActionBuilder To(string to) {
            _action.To = to;
            return this;
        }

        public EntityBuilder Entity(string name) {
            return _builder.Entity(name);
        }

        public SearchTypeBuilder SearchType(string name) {
            return _builder.SearchType(name);
        }

        public MapBuilder Map(string name) {
            return _builder.Map(name);
        }

        public ProcessBuilder TemplatePath(string path) {
            return _builder.TemplatePath(path);
        }

        public ProcessBuilder ScriptPath(string path) {
            return _builder.ScriptPath(path);
        }

        public ProcessConfigurationElement Process() {
            return _builder.Process();
        }
    }
}