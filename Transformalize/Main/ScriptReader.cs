using System.Collections.Generic;
using System.IO;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Logging;

namespace Transformalize.Main {
    public class ScriptReader {

        private readonly ScriptElementCollection _elements;
        private readonly char[] _s = new[] { '\\' };

        public ScriptReader(ScriptElementCollection elements) {
            _elements = elements;
        }

        public Dictionary<string, Script> Read() {
            var scriptElements = _elements.Cast<ScriptConfigurationElement>().ToArray();
            var path = _elements.Path == Common.DefaultValue ? string.Empty : _elements.Path;

            var scripts = new Dictionary<string, Script>();

            foreach (var script in scriptElements) {
                var fileInfo = path.Equals(string.Empty) ? new FileInfo(script.File) : new FileInfo(path.TrimEnd(_s) + @"\" + script.File);
                if (!fileInfo.Exists) {
                    TflLogger.Warn(string.Empty, string.Empty, "Missing Script: {0}.", fileInfo.FullName);
                } else {
                    scripts[script.Name] = new Script(script.Name, File.ReadAllText(fileInfo.FullName), fileInfo.FullName);
                    TflLogger.Debug(string.Empty, string.Empty, "Loaded script {0}.", fileInfo.FullName);
                }
            }

            return scripts;

        }
    }
}
