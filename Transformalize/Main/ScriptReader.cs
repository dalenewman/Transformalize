using System;
using System.Collections.Generic;
using System.IO;
using Transformalize.Configuration;
using Transformalize.Logging;
using Transformalize.Runner;

namespace Transformalize.Main {
    public class ScriptReader {

        private readonly List<TflScript> _elements;
        private readonly ILogger _logger;
        private readonly char[] _s = new[] { '\\' };

        public ScriptReader(List<TflScript> elements, ILogger logger) {
            _elements = elements;
            _logger = logger;
        }

        public Dictionary<string, Script> Read() {
            var scriptElements = _elements;

            var scripts = new Dictionary<string, Script>();

            foreach (var script in scriptElements) {

                if (script.File == string.Empty && script.Script != string.Empty) {
                    scripts[script.Name] = new Script(script.Name, script.Script, "none");
                    continue;
                }

                var resource = script.Path.Equals(string.Empty) ? script.File : script.Path.TrimEnd(_s) + @"\" + script.File;
                if (resource.StartsWith("http", StringComparison.OrdinalIgnoreCase)) {
                    var contents = new ContentsWebReader(_logger).Read(resource);
                    scripts[script.Name] = new Script(script.Name, contents.Content, contents.FileName);
                } else {
                    var fileInfo = new FileInfo(resource);
                    if (!fileInfo.Exists) {
                        _logger.Warn("Missing Script: {0}.", fileInfo.FullName);
                    } else {
                        scripts[script.Name] = new Script(script.Name, File.ReadAllText(fileInfo.FullName), fileInfo.FullName);
                        _logger.Debug("Loaded script {0}.", fileInfo.FullName);
                    }
                }
            }

            return scripts;

        }
    }
}
