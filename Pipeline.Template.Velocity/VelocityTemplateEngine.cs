#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cfg.Net.Contracts;
using NVelocity;
using NVelocity.Runtime;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Template.Velocity {

    public static class VelocityInitializer {
        private static readonly object Locker = new object();
        private static bool Initialized { get; set; }

        public static void Init() {
            lock (Locker) {
                if (Initialized)
                    return;

                NVelocity.App.Velocity.SetProperty(RuntimeConstants.RUNTIME_LOG_LOGSYSTEM, typeof(VelocityLogSystem).FullName);
                NVelocity.App.Velocity.Init();
                Initialized = true;
            }

        }
    }

    public class VelocityTemplateEngine : ITemplateEngine {

        private readonly PipelineContext _context;
        private readonly Configuration.Template _template;

        // Using Cfg-NET's "Reader" to read content, files, or web addresses with possible parameters.
        private readonly IReader _templateReader;

        public VelocityTemplateEngine(PipelineContext context, Configuration.Template template, IReader templateReader) {

            VelocityInitializer.Init();

            _context = context;
            _template = template;
            _templateReader = templateReader;

        }

        public string Render() {

            var p = new Dictionary<string, string>();
            var l = new Cfg.Net.Loggers.MemoryLogger();

            // get template
            _context.Debug(() => $"Reading {_template.File}");
            var templateContent = _templateReader.Read(_template.File, p, l);

            if (l.Errors().Any()) {
                foreach (var error in l.Errors()) {
                    _context.Error(error);
                }
                return string.Empty;
            }


            try {
                _context.Debug(() => $"Compiling {_template.Name}.");

                var context = new VelocityContext();
                context.Put("Process", _context.Process);
                foreach (var parameter in _template.Parameters) {
                    context.Put(parameter.Name, Constants.ConversionMap[parameter.Type](parameter.Value));
                }
                if (p.Any()) {
                    foreach (var parameter in p) {
                        context.Put(parameter.Key, parameter.Value);
                    }
                }

                var sb = new StringBuilder();
                using (var sw = new StringWriter(sb)) {
                    NVelocity.App.Velocity.Evaluate(context, sw, string.Empty, templateContent);
                    sw.Flush();
                }
                return sb.ToString();

            } catch (Exception ex) {
                _context.Error($"Error parsing template {_template.Name}.");
                _context.Error(ex, ex.Message.Replace("{", "{{").Replace("}", "}}"));
                return string.Empty;
            }
        }

    }
}
