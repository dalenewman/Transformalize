namespace Transformalize.Configuration.Builders {
    public class ScriptBuilder {
        private readonly ProcessBuilder _processBuilder;
        private readonly TflScript _script;

        public ScriptBuilder(ProcessBuilder processBuilder, TflScript script) {
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

        public TflProcess Process() {
            return _processBuilder.Process();
        }
    }
}