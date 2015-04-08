using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Transformalize.Configuration;
using Transformalize.Logging;
using Transformalize.Runner;

namespace Transformalize.Main {
    public class ScriptReader {

        private readonly List<TflScript> _elements;
        private readonly char[] _s = new[] { '\\' };

        public ScriptReader(List<TflScript> elements) {
            _elements = elements;
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
                    var contents = new ContentsWebReader().Read(resource);
                    scripts[script.Name] = new Script(script.Name, contents.Content, contents.FileName);
                } else {
                    var fileInfo = new FileInfo(resource);
                    if (!fileInfo.Exists) {
                        TflLogger.Warn(string.Empty, string.Empty, "Missing Script: {0}.", fileInfo.FullName);
                    } else {
                        scripts[script.Name] = new Script(script.Name, File.ReadAllText(fileInfo.FullName), fileInfo.FullName);
                        TflLogger.Debug(string.Empty, string.Empty, "Loaded script {0}.", fileInfo.FullName);
                    }
                }
            }

            return scripts;

        }
    }
}
