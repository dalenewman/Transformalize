using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Microsoft.AnalysisServices;
using Transformalize.Configuration;
using Transformalize.Extensions;
using Transformalize.Libs.Ninject;
using Transformalize.Libs.NVelocity.App;
using Transformalize.Libs.RazorEngine;
using Transformalize.Logging;
using Transformalize.Main.Providers;
using Transformalize.Main.Providers.File;
using Transformalize.Main.Transform;

namespace Transformalize.Main {

    public static class ProcessFactory {

        public static Process[] Create(string resource, Options options = null, Dictionary<string, string> parameters = null) {
            var source = ConfigurationFactory.DetermineConfigurationSource(resource);
            CombineParameters(source, resource, parameters);
            TflLogger.Info(string.Empty, string.Empty, "Process Requested from {0}", source.ToString());
            return Create(new ConfigurationFactory(resource, parameters).Create(), options);
        }

        public static Process CreateSingle(string resource, Options options = null, Dictionary<string, string> parameters = null) {
            var source = ConfigurationFactory.DetermineConfigurationSource(resource);
            CombineParameters(source, resource, parameters);
            TflLogger.Info(string.Empty, string.Empty, "Process Requested from {0}", source.ToString());
            return CreateSingle(new ConfigurationFactory(resource, parameters).Create()[0], options);
        }
        /// <summary>
        /// Create process(es) from a configuration element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static Process[] Create(TflRoot element, Options options = null) {
            return Create(element.Processes, options);
        }

        private static Process[] Create(List<TflProcess> elements, Options options = null) {

            var host = System.Net.Dns.GetHostName();
            var process = string.Empty;
            if (elements != null && elements.Count > 0) {
                process = string.Join(", ", elements.Select(p => p.Name));
            }
            var ip4 = System.Net.Dns.GetHostEntry(host).AddressList.Where(a => a.ToString().Length > 4 && a.ToString()[4] != ':').Select(a => a.ToString()).ToArray();
            TflLogger.Info(process, string.Empty, "Host is {0} {1}", host, string.Join(", ", ip4.Any() ? ip4 : new[] { string.Empty }));

            var processes = new List<Process>();
            if (options == null) {
                options = new Options();
            }

            if (elements != null) {
                processes.AddRange(elements.Select(element => new ProcessReader(element, ref options).Read()));
            }

            return processes.ToArray();
        }

        public static Process CreateSingle(TflProcess process, Options options = null) {
            return Create(new List<TflProcess> { process }, options)[0];
        }

        private static void CombineParameters(ConfigurationSource source, string resource, Dictionary<string, string> parameters) {
            if (source == ConfigurationSource.Xml || resource.IndexOf('?') <= 0)
                return;

            if (parameters == null) {
                parameters = new Dictionary<string, string>();
            }
            foreach (var pair in Common.ParseQueryString(resource.Substring(resource.IndexOf('?')))) {
                parameters[pair.Key] = pair.Value;
            }
        }

        private class ProcessReader {

            private readonly TflProcess _element;
            private readonly Options _options;
            private readonly string _processName = string.Empty;
            private Process _process;
            private readonly string[] _transformToFields = { "fromxml", "fromregex", "fromjson", "fromsplit" };

            public ProcessReader(TflProcess process, ref Options options) {
                ShortHandFactory.ExpandShortHandTransforms(process);
                _element = Adapt(process, _transformToFields);
                _processName = process.Name;
                _options = options;
            }

            public Process Read() {

                if (_element == null) {
                    throw new TransformalizeException(string.Empty, string.Empty, "Can't find a process named {0}.", _processName);
                }
                _process = new Process(_element.Name) {
                    Options = _options,
                    TemplateContentType = _element.TemplateContentType.Equals("raw") ? Encoding.Raw : Encoding.Html,
                    Enabled = _element.Enabled,
                    FileInspectionRequest = _element.FileInspection.Count == 0 ? new FileInspectionRequest() : _element.FileInspection[0].GetInspectionRequest(),
                    Star = _element.Star,
                    View = _element.View,
                    Mode = _element.Mode,
                    StarEnabled = _element.StarEnabled,
                    TimeZone = string.IsNullOrEmpty(_element.TimeZone) ? TimeZoneInfo.Local.Id : _element.TimeZone,
                    PipelineThreading = (PipelineThreading)Enum.Parse(typeof(PipelineThreading), _element.PipelineThreading, true),
                    Parallel = _element.Parallel,
                    Kernal = new StandardKernel(new NinjectBindings(_element))
                };

                // options mode overrides process node
                if (_options.Mode != Common.DefaultValue && _options.Mode != _process.Mode) {
                    _process.Mode = _options.Mode;
                }
                TflLogger.Info(_processName, string.Empty, "Mode is {0}", _process.Mode);

                //shared across the process
                var connectionFactory = new ConnectionFactory(_process);
                _process.Connections = connectionFactory.Create(_element.Connections);
                if (!_process.Connections.ContainsKey("output")) {
                    TflLogger.Warn(_processName, string.Empty, "No output connection detected.  Defaulting to internal.");
                    var output = _element.GetDefaultOf<TflConnection>();
                    output.Name = "output";
                    output.Provider = "internal";
                    _process.OutputConnection = connectionFactory.Create(output);
                } else {
                    _process.OutputConnection = _process.Connections["output"];
                }

                //logs set after connections, because they may depend on them
                LoadLogConfiguration(_element, ref _process);

                _process.Scripts = new ScriptReader(_element.Scripts).Read();
                _process.Actions = new ActionReader(_process).Read(_element.Actions);
                _process.Templates = new TemplateReader(_process, _element.Templates).Read();
                _process.SearchTypes = new SearchTypeReader(_element.SearchTypes).Read();
                new MapLoader(ref _process, _element.Maps).Load();

                if (_process.Templates.Any(t => t.Value.Engine.Equals("velocity", StringComparison.OrdinalIgnoreCase))) {
                    Velocity.Init();
                    _process.VelocityInitialized = true;
                }

                //these depend on the shared process properties
                new EntitiesLoader(ref _process, _element.Entities).Load();
                new OperationsLoader(ref _process, _element.Entities).Load();

                _process.Relationships = new RelationshipsReader(_process, _element.Relationships).Read();
                new ProcessOperationsLoader(ref _process, _element.CalculatedFields).Load();
                new EntityRelationshipLoader(ref _process).Load();

                return _process;
            }

            private void LoadLogConfiguration(TflProcess element, ref Process process) {

                process.LogRows = element.Log.Any() ? (element.Log[0].Rows == 0 ? 10000 : element.Log[0].Rows) : 10000;

                if (element.Log.Count == 0)
                    return;

                var fileLogs = new List<Log>();

                foreach (var logElement in element.Log) {
                    var log = MapLog(process, logElement);
                    if (log.Provider == ProviderType.File) {
                        fileLogs.Add(log);
                    } else {
                        process.Log.Add(log);
                    }
                }

                process.Log.AddRange(fileLogs);
            }

            private Log MapLog(Process process, TflLog logElement) {
                var log = new Log {
                    Name = logElement.Name,
                    Subject = logElement.Subject,
                    From = logElement.From,
                    To = logElement.To,
                    Layout = logElement.Layout,
                    File = logElement.File,
                    Folder = logElement.Folder,
                    Async = logElement.Async
                };

                if (logElement.Connection != Common.DefaultValue) {
                    if (process.Connections.ContainsKey(logElement.Connection)) {
                        log.Connection = process.Connections[logElement.Connection];
                        if (log.Connection.Type == ProviderType.File && log.File.Equals(Common.DefaultValue)) {
                            log.File = log.Connection.File;
                        }
                    } else {
                        throw new TransformalizeException(process.Name, string.Empty, "You are referencing an invalid connection name in your log configuration.  {0} is not confiured in <connections/>.", logElement.Connection);
                    }
                }

                try {
                    EventLevel eventLevel;
                    if (Enum.TryParse(logElement.Level, out eventLevel)) {
                        log.Level = eventLevel;
                    } else {
                        switch (logElement.Level.ToLower().Left(4)) {
                            case "debu":
                                log.Level = EventLevel.Verbose;
                                break;
                            case "info":
                                log.Level = EventLevel.Informational;
                                break;
                            case "warn":
                                log.Level = EventLevel.Warning;
                                break;
                            case "erro":
                                log.Level = EventLevel.Error;
                                break;
                            default:
                                log.Level = EventLevel.Informational;
                                TflLogger.Warn(_processName, string.Empty, "Invalid log level: {0}.  Valid values are Informational, Error, Verbose, and Warning. Defaulting to Informational.", logElement.Level);
                                break;
                        }

                    }
                    log.Provider = (ProviderType)Enum.Parse(typeof(ProviderType), logElement.Provider, true);
                } catch (Exception ex) {
                    throw new TransformalizeException(process.Name, string.Empty, "Log configuration invalid. {0}", ex.Message);
                }
                return log;
            }

            private static TflProcess Adapt(TflProcess process, IEnumerable<string> transformToFields) {

                foreach (var field in transformToFields) {
                    while (new TransformFieldsToParametersAdapter(process).Adapt(field) > 0) {
                        new TransformFieldsMoveAdapter(process).Adapt(field);
                    };
                }

                return process;
            }

        }

    }
}
