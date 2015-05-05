using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Extensions;
using Transformalize.Libs.NVelocity.App;
using Transformalize.Libs.RazorEngine;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Logging;
using Transformalize.Main.Providers;
using Transformalize.Main.Providers.File;

namespace Transformalize.Main {

    public static class ProcessFactory {

        private static Process[] Create(List<TflProcess> tflProcesses, ILogger logger, Options options = null) {

            logger.Info(GetHost());

            var processes = new List<Process>();
            options = options ?? new Options();

            foreach (var process in tflProcesses) {
                var kernal = process.Register(logger);
                process.Resolve(kernal);
                processes.Add(new ProcessReader(process, logger, options).Read());
            }

            return processes.ToArray();
        }

        public static Process[] Create(string resource, ILogger logger, Options options = null, Dictionary<string, string> parameters = null) {
            var source = ConfigurationFactory.DetermineConfigurationSource(resource);
            CombineParameters(source, resource, parameters);
            logger.Info("Process Requested from {0}", source.ToString());
            return Create(new ConfigurationFactory(resource, logger, parameters).Create(), logger, options);
        }
        
        public static Process CreateSingle(string resource, ILogger logger, Options options = null, Dictionary<string, string> parameters = null) {
            var source = ConfigurationFactory.DetermineConfigurationSource(resource);
            CombineParameters(source, resource, parameters);
            logger.Info("Process Requested from {0}", source.ToString());
            return CreateSingle(new ConfigurationFactory(resource, logger, parameters).Create()[0], logger, options);
        }
        /// <summary>
        /// Create process(es) from a configuration element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static Process[] Create(TflRoot element, ILogger logger, Options options = null) {
            return Create(element.Processes, logger, options);
        }

        public static Process CreateSingle(TflProcess process, ILogger logger, Options options = null) {
            return Create(new List<TflProcess> { process }, logger, options)[0];
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

        private static string GetHost() {
            var host = System.Net.Dns.GetHostName();
            var ip4 = System.Net.Dns.GetHostEntry(host).AddressList.Where(a => a.ToString().Length > 4 && a.ToString()[4] != ':').Select(a => a.ToString()).ToArray();
            return string.Format("Host is {0} {1}", host, string.Join(", ", ip4.Any() ? ip4 : new[] { string.Empty }));
        }


        private class ProcessReader {
            private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;

            private readonly TflProcess _element;
            private readonly ILogger _logger;
            private readonly Options _options;
            private Process _process;

            public ProcessReader(TflProcess process, ILogger logger, Options options) {
                _element = process;
                _logger = logger;
                _options = options;
            }

            public Process Read() {

                _process = new Process(_element.Name, _logger) {
                    Options = _options,
                    TemplateContentType = _element.TemplateContentType.Equals("raw") ? Encoding.Raw : Encoding.Html,
                    Enabled = _element.Enabled,
                    FileInspectionRequest = _element.FileInspection.Count == 0 ? new FileInspectionRequest(_element.Name) : _element.FileInspection[0].GetInspectionRequest(_element.Name),
                    Star = _element.Star,
                    View = _element.View,
                    Mode = _element.Mode,
                    StarEnabled = _element.StarEnabled,
                    ViewEnabled = _element.ViewEnabled,
                    TimeZone = string.IsNullOrEmpty(_element.TimeZone) ? TimeZoneInfo.Local.Id : _element.TimeZone,
                    PipelineThreading = (PipelineThreading)Enum.Parse(typeof(PipelineThreading), _element.PipelineThreading, true),
                    Parallel = _element.Parallel
                };

                // options mode overrides process mode
                if (_options.Mode != Common.DefaultValue && _options.Mode != _process.Mode) {
                    _process.Mode = _options.Mode;
                }
                _logger.Info("{0} entit{1} in {2} mode.", _element.Entities.Count, _element.Entities.Count.Pluralize(), _process.Mode == string.Empty ? "Default" : _process.Mode);
                _logger.Info("Running {0} with a {1} pipeline.", _element.Parallel ? "Parallel" : "Serial", GetPipelineDescription());

                //shared across the process
                _process.Connections.AddRange(_element.Connections);
                _process.OutputConnection = _process.Connections.Output().Connection;

                _process.DataSets = GetDataSets(_element);
                _process.Scripts = new ScriptReader(_element.Scripts, _logger).Read();
                _process.Actions = new ActionReader(_process).Read(_element.Actions);
                _process.Templates = new TemplateReader(_process, _element.Templates).Read();
                _process.SearchTypes = new SearchTypeReader(_element.SearchTypes).Read();
                new MapLoader(ref _process, _element.Maps).Load();

                //these depend on the shared process properties
                new EntitiesLoader(ref _process, _element.Entities).Load();
                new OperationsLoader(ref _process, _element.Entities).Load();

                _process.Relationships = new RelationshipsReader(_process, _element.Relationships).Read();
                new ProcessOperationsLoader(_process, _element.CalculatedFields).Load();
                new EntityRelationshipLoader(_process).Load();

                return _process;
            }

            private string GetPipelineDescription() {
                var pipeline = _element.PipelineThreading;
                if (pipeline == "Default") {
                    if (_element.Entities.All(e => e.PipelineThreading == "SingleThreaded")) {
                        pipeline = "SingleThreaded";
                    } else if (_element.Entities.All(e => e.PipelineThreading == "MultiThreaded")) {
                        pipeline = "MultiThreaded";
                    } else {
                        pipeline = "Mixed";
                    }
                }
                return pipeline;
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

        }

    }
}
