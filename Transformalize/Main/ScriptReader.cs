using System.Collections.Generic;
using System.IO;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Libs.NLog;

namespace Transformalize.Main {
    public class ScriptReader
    {

        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly ScriptElementCollection _elements;
        private readonly char[] _s = new[] {'\\'};

        public ScriptReader(ScriptElementCollection elements)
        {
            _elements = elements;
        }

        public Dictionary<string, Script> Read()
        {
            var scriptElements = _elements.Cast<ScriptConfigurationElement>().ToArray();
            var path = _elements.Path;

            var scripts = new Dictionary<string, Script>();

            foreach (var script in scriptElements) {
                var fileInfo = new FileInfo(path.TrimEnd(_s) + @"\" + script.File);
                if (!fileInfo.Exists) {
                    _log.Warn("Missing Script: {0}.", fileInfo.FullName);
                } else {
                    scripts[script.Name] = new Script(script.Name, File.ReadAllText(fileInfo.FullName), fileInfo.FullName);
                    _log.Debug("Loaded script {0}.", fileInfo.FullName);
                }
            }

            return scripts;

        } 
    }
}
