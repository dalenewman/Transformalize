using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Extensions;
using Transformalize.Libs.RazorEngine;
using Transformalize.Libs.RazorEngine.Configuration.Fluent;
using Transformalize.Libs.RazorEngine.Templating;
using Transformalize.Main.Parameters;
using Transformalize.Runner;

namespace Transformalize.Main {

    public class TemplateReader {
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;

        private readonly Process _process;
        private readonly List<TflTemplate> _elements;
        private readonly DefaultFactory _defaultFactory;

        public TemplateReader(Process process, List<TflTemplate> elements) {
            _process = process;
            _elements = elements;
            _defaultFactory = new DefaultFactory(process.Logger);
        }

        public Dictionary<string, Template> Read() {

            var templateElements = _elements;
            var templates = new Dictionary<string, Template>();

            foreach (var element in templateElements) {

                if (!element.Enabled)
                    continue;

                var reader = element.File.StartsWith("http", IC) ? (ContentsReader)new ContentsWebReader(_process.Logger) : new ContentsFileReader(element.Path, _process.Logger);
                var template = new Template(_process, element, reader.Read(element.File));

                foreach (var parameter in element.Parameters) {
                    template.Parameters[parameter.Name] = new Parameter(parameter.Name, _defaultFactory.Convert(parameter.Value, parameter.Type));
                }

                foreach (var action in element.Actions) {
                    var modes = action.GetModes();
                    if (modes.Length > 0 && !modes.Contains("*") && !modes.Any(m => m.Equals(_process.Mode, IC)))
                        continue;

                    var templateAction = new TemplateAction(_process, template.Name, action);
                    if (!String.IsNullOrEmpty(action.Connection)) {
                        templateAction.Connection = _process.Connections.GetConnectionByName(action.Connection).Connection;
                    }

                    template.Actions.Add(templateAction);
                }

                templates[element.Name] = template;
                _process.Logger.Debug("Loaded template {0} with {1} parameter{2}.", element.File, template.Parameters.Count, template.Parameters.Count.Plural());

            }

            return templates;

        }

    }
}
