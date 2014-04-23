using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Libs.NLog;
using Transformalize.Libs.RazorEngine;
using Transformalize.Libs.RazorEngine.Configuration.Fluent;
using Transformalize.Libs.RazorEngine.Templating;
using Transformalize.Runner;

namespace Transformalize.Main {

    public class TemplateReader {
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;

        private readonly Logger _log = LogManager.GetLogger(string.Empty);
        private readonly Process _process;
        private readonly TemplateElementCollection _elements;
        private readonly DefaultFactory _defaultFactory = new DefaultFactory();

        public TemplateReader(Process process, TemplateElementCollection elements) {
            _process = process;
            _elements = elements;
            SetupRazorTemplateService();
        }

        public Dictionary<string, Template> Read() {

            var templateElements = _elements.Cast<TemplateConfigurationElement>().ToArray();
            var path = _elements.Path;
            var templates = new Dictionary<string, Template>();

            foreach (var element in templateElements) {

                var reader = element.File.StartsWith("http", IC) ? (IContentsReader) new ContentsWebReader() : new ContentsFileReader(path);
                var contents = element.Render ? reader.Read(element.File) : new Contents() {FileName = element.File};
                var template = new Template(_process, element, contents);

                foreach (SettingConfigurationElement setting in element.Settings) {
                    template.Settings[setting.Name] = _defaultFactory.Convert(setting.Value, setting.Type);
                }

                foreach (ActionConfigurationElement action in element.Actions) {
                    var modes = GetModes(action).ToList();
                    if (modes.Count != 0 && !modes.Contains("*") && !modes.Any(m => m.Equals(_process.Options.Mode, IC)))
                        continue;

                    var templateAction = new TemplateAction {
                        Action = action.Action,
                        File = action.File,
                        Method = action.Method,
                        Url = action.Url,
                        TemplateName = template.Name,
                        From = action.From,
                        Arguments = action.Arguments,
                        Bcc = action.Bcc,
                        Cc = action.Cc,
                        EnableSsl = action.EnableSsl,
                        Html = action.Html,
                        Password = action.Password,
                        Port = action.Port,
                        Subject = action.Subject,
                        To = action.To,
                        Username = action.Username,
                        Host = action.Host,
                        Body = action.Body,
                        Modes = modes
                    };

                    if (!String.IsNullOrEmpty(action.Connection)) {
                        if (_process.Connections.ContainsKey(action.Connection)) {
                            templateAction.Connection = _process.Connections[action.Connection];
                        } else {
                            var message = string.Format("The template '{0}' refers to an invalid connection named '{1}'.", action.Action, action.Connection);
                            _log.Error(message);
                            throw new TransformalizeException(message);
                        }
                    }

                    template.Actions.Add(templateAction);
                }

                templates[element.Name] = template;
                _log.Debug("Loaded template {0} with {1} setting{2}.", element.File, template.Settings.Count, template.Settings.Count == 1 ? string.Empty : "s");

            }

            return templates;

        }

        private static IEnumerable<string> GetModes(ActionConfigurationElement action) {
            var modes = new List<string>();

            if (action.Mode != string.Empty) {
                modes.Add(action.Mode);
            }

            modes.AddRange(from ModeConfigurationElement mode in action.Modes select mode.Mode);
            return modes;
        }

        private void SetupRazorTemplateService() {
            const Encoding theDefault = Encoding.Html;

            if (_process.TemplateContentType == theDefault)
                return;

            var config = new FluentTemplateServiceConfiguration(c => c.WithEncoding(_process.TemplateContentType));
            var templateService = new TemplateService(config);
            Razor.SetTemplateService(templateService);
            _log.Debug("Set RazorEngine to {0} content type.", _process.TemplateContentType);
        }
    }
}
