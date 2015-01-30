using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Extensions;
using Transformalize.Libs.RazorEngine;
using Transformalize.Libs.RazorEngine.Configuration.Fluent;
using Transformalize.Libs.RazorEngine.Templating;
using Transformalize.Logging;
using Transformalize.Main.Parameters;
using Transformalize.Runner;

namespace Transformalize.Main {

    public class TemplateReader {
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;

        private readonly Process _process;
        private readonly List<TflTemplate> _elements;
        private readonly DefaultFactory _defaultFactory = new DefaultFactory();

        public TemplateReader(Process process, List<TflTemplate> elements) {
            _process = process;
            _elements = elements;
            SetupRazorTemplateService();
        }

        public Dictionary<string, Template> Read() {

            var templateElements = _elements;
            var templates = new Dictionary<string, Template>();

            foreach (var element in templateElements) {

                if (!element.Enabled)
                    continue;

                var reader = element.File.StartsWith("http", IC) ? (ContentsReader)new ContentsWebReader() : new ContentsFileReader(element.Path);
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
                        if (_process.Connections.ContainsKey(action.Connection)) {
                            templateAction.Connection = _process.Connections[action.Connection];
                        } else {
                            var message = string.Format("The template '{0}' refers to an invalid connection named '{1}'.", action.Action, action.Connection);
                            throw new TransformalizeException(_process.Name, string.Empty, message);
                        }
                    }

                    template.Actions.Add(templateAction);
                }

                templates[element.Name] = template;
                TflLogger.Debug(_process.Name, string.Empty, "Loaded template {0} with {1} parameter{2}.", element.File, template.Parameters.Count, template.Parameters.Count.Plural());

            }

            return templates;

        }

        private void SetupRazorTemplateService() {
            const Encoding theDefault = Encoding.Html;

            if (_process.TemplateContentType == theDefault)
                return;

            var config = new FluentTemplateServiceConfiguration(c => c.WithEncoding(_process.TemplateContentType));
            var templateService = new TemplateService(config);
            Razor.SetTemplateService(templateService);
            TflLogger.Debug(_process.Name, string.Empty, "Set RazorEngine to {0} content type.", _process.TemplateContentType);
        }
    }
}
