using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Libs.NLog;
using Transformalize.Libs.RazorEngine;
using Transformalize.Libs.RazorEngine.Configuration.Fluent;
using Transformalize.Libs.RazorEngine.Templating;

namespace Transformalize.Main {

    public class TemplateReader {
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;

        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly Process _process;
        private readonly TemplateElementCollection _elements;
        private readonly DefaultFactory _defaultFactory = new DefaultFactory();
        private readonly char[] _s = new[] { '\\' };

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
                var fileInfo = new FileInfo(path.TrimEnd(_s) + @"\" + element.File);
                if (!fileInfo.Exists) {
                    _log.Warn("Missing Template {0}.", fileInfo.FullName);
                } else {
                    var template = new Template(_process, element, File.ReadAllText(fileInfo.FullName), fileInfo.FullName);

                    foreach (SettingConfigurationElement setting in element.Settings) {
                        template.Settings[setting.Name] = _defaultFactory.Convert(setting.Value, setting.Type);
                    }

                    foreach (ActionConfigurationElement action in element.Actions) {
                        var modes = GetModes(action).ToList();
                        if (modes.Contains("*") || modes.Any(m => m.Equals(_process.Options.Mode, IC))) {

                            var templateAction = new TemplateAction {
                                Action = action.Action,
                                File = action.File,
                                Method = action.Method,
                                Url = action.Url,
                                TemplateName = template.Name,
                                Modes = modes
                            };

                            if (!String.IsNullOrEmpty(action.Connection) &&
                                _process.Connections.ContainsKey(action.Connection)) {
                                templateAction.Connection = _process.Connections[action.Connection];
                            }

                            template.Actions.Add(templateAction);
                        }
                    }

                    templates[element.Name] = template;
                    _log.Debug("Loaded template {0} with {1} setting{2}.", fileInfo.FullName, template.Settings.Count, template.Settings.Count == 1 ? string.Empty : "s");
                }
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
