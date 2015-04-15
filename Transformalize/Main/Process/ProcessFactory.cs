using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design;
using Transformalize.Libs.Ninject;
using Transformalize.Libs.NVelocity.App;
using Transformalize.Libs.RazorEngine;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Logging;
using Transformalize.Main.Providers;
using Transformalize.Main.Providers.File;

namespace Transformalize.Main {

    public static class ProcessFactory {

        private static Process[] Create(List<TflProcess> tflProcesses, Options options = null) {

            TflLogger.LogHost(tflProcesses);

            var processes = new List<Process>();
            options = options ?? new Options();

            foreach (var process in tflProcesses) {
                var kernal = process.Register();
                process.Resolve(kernal);
                processes.Add(new ProcessReader(process, options).Read());
            }

            return processes.ToArray();
        }

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

            public ProcessReader(TflProcess process, Options options) {
                _element = process;
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
                    FileInspectionRequest = _element.FileInspection.Count == 0 ? new FileInspectionRequest(_element.Name) : _element.FileInspection[0].GetInspectionRequest(_element.Name),
                    Star = _element.Star,
                    View = _element.View,
                    Mode = _element.Mode,
                    StarEnabled = _element.StarEnabled,
                    TimeZone = string.IsNullOrEmpty(_element.TimeZone) ? TimeZoneInfo.Local.Id : _element.TimeZone,
                    PipelineThreading = (PipelineThreading)Enum.Parse(typeof(PipelineThreading), _element.PipelineThreading, true),
                    Parallel = _element.Parallel
                };

                // options mode overrides process node
                if (_options.Mode != Common.DefaultValue && _options.Mode != _process.Mode) {
                    _process.Mode = _options.Mode;
                }
                TflLogger.Info(_processName, string.Empty, "Mode is {0}", _process.Mode);

                //shared across the process
                _process.Connections = _element.Connections.ToDictionary(c => c.Name, c => c.Connection);
                _process.OutputConnection = _process.Connections["output"];

                //logs set after connections, because they may depend on them
                LoadLogConfiguration(_element, ref _process);

                _process.DataSets = GetDataSets(_element);
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

            private static Dictionary<string, List<Row>> GetDataSets(TflProcess process) {

                var dataSets = new Dictionary<string, List<Row>>();
                if (!process.DataSets.Any()) 
                    return dataSets;

                foreach (var dataSet in process.DataSets) {
                    dataSets[dataSet.Name] = new List<Row>();
                    var rows = dataSets[dataSet.Name];
                    foreach (var r in dataSet.Rows) {
                        var row = new Row();
                        foreach (var pair in r) {
                            row[pair.Key] = pair.Value;
                        }
                        rows.Add(row);
                    }
                }
                return dataSets;
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
                    log.Connection = process.Connections[logElement.Connection];
                    if (log.Connection.Type == ProviderType.File && log.File.Equals(Common.DefaultValue)) {
                        log.File = log.Connection.File;
                    }
                }

                try {
                    EventLevel eventLevel;
                    if (Enum.TryParse(logElement.Level, out eventLevel)) {
                        log.Level = eventLevel;
                    } else {
                        log.Level = EventLevel.Informational;
                        TflLogger.Warn(_processName, string.Empty, "Invalid log level: {0}.  Valid values are Informational, Error, Verbose, and Warning. Defaulting to Informational.", logElement.Level);
                    }
                    log.Provider = (ProviderType)Enum.Parse(typeof(ProviderType), logElement.Provider, true);
                } catch (Exception ex) {
                    throw new TransformalizeException(process.Name, string.Empty, "Log configuration invalid. {0}", ex.Message);
                }
                return log;
            }

        }

    }
}
