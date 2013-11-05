namespace Transformalize.Configuration.Builders {
    public class ScriptBuilder {
        private readonly ProcessBuilder _processBuilder;
        private readonly ScriptConfigurationElement _script;

        public ScriptBuilder(ProcessBuilder processBuilder, ScriptConfigurationElement script) {
            _processBuilder = processBuilder;
            _script = script;
        }

        public ScriptBuilder File(string file) {
            _script.File = file;
            return this;
        }

        public ScriptBuilder Script(string name) {
            return _processBuilder.Script(name);
        }

        public ProcessConfigurationElement Process() {
            return _processBuilder.Process();
        }
    }
}