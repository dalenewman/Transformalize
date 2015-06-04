using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Extensions;
using Transformalize.Libs.RazorEngine;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Logging;
using Transformalize.Main.Providers.File;
using Transformalize.Runner;

namespace Transformalize.Main {

    public static class ProcessFactory {

        private static Process[] Create(IEnumerable<TflProcess> tflProcesses, ILogger logger, Options options = null) {

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
            logger.Info("Process Requested from {0}", source.ToString());

            return Create(
                new ConfigurationFactory(
                    resource, 
                    logger, 
                    CombineParameters(source, resource, parameters)
                ).Create(), 
                logger, 
                options
            );
        }

        public static Process CreateSingle(string resource, ILogger logger, Options options = null, Dictionary<string, string> parameters = null) {
            var source = ConfigurationFactory.DetermineConfigurationSource(resource);
            logger.Info("Process Requested from {0}", source.ToString());

            return CreateSingle(
                new ConfigurationFactory(
                    resource, 
                    logger, 
                    CombineParameters(source, resource, parameters)
                ).Create()[0], 
                logger, 
                options
            );
        }

        /// <summary>
        /// Create process(es) from a configuration element.
        /// </summary>
        /// <param name="element">The Cfg-NET configuration element.</param>
        /// <param name="logger">An ILogger implementation.</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static Process[] Create(TflRoot element, ILogger logger, Options options = null) {
            return Create(element.Processes, logger, options);
        }

        public static Process CreateSingle(TflProcess process, ILogger logger, Options options = null) {
            return Create(new List<TflProcess> { process }, logger, options)[0];
        }

        private static Dictionary<string, string> CombineParameters(ConfigurationSource source, string resource, Dictionary<string, string> parameters) {
            if (source == ConfigurationSource.Xml || resource.IndexOf('?') <= 0)
                return parameters;

            if (parameters == null) {
                parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
            foreach (var pair in Common.ParseQueryString(resource.Substring(resource.IndexOf('?')))) {
                parameters[pair.Key] = pair.Value;
            }
            return parameters;
        }

        private class ProcessReader {

            private readonly TflProcess _configuration;
            private readonly ILogger _logger;
            private readonly Options _options;
            private Process _process;
            private readonly IProcessRunner _runner;

            public ProcessReader(TflProcess process, ILogger logger, Options options) {
                _configuration = process;
                _logger = logger;
                _options = options;
                _runner = GetRunner(DetermineMode());
            }

            private string DetermineMode() {
                // options mode overrides process mode
                if (_options.Mode != Common.DefaultValue && _options.Mode != _configuration.Mode) {
                    _configuration.Mode = _options.Mode;
                }
                return _configuration.Mode;
            }

            private static IProcessRunner GetRunner(string mode) {
                switch (mode) {
                    case "init":
                        return new InitializeRunner();
                    case "metadata":
                        return new MetadataRunner();
                    default:
                        return new ProcessRunner();
                }
            }

            public Process Read() {

                _process = new Process(_configuration.Name, _logger) {
                    Configuration = _configuration,
                    Options = _options,
                    TemplateContentType = _configuration.TemplateContentType.Equals("raw") ? Encoding.Raw : Encoding.Html,
                    Enabled = _configuration.Enabled,
                    FileInspectionRequest = _configuration.FileInspection.Count == 0 ? new FileInspectionRequest(_configuration.Name) : _configuration.FileInspection[0].GetInspectionRequest(_configuration.Name),
                    Star = _configuration.Star,
                    View = _configuration.View,
                    Mode = _configuration.Mode,
                    StarEnabled = _configuration.StarEnabled,
                    ViewEnabled = _configuration.ViewEnabled,
                    TimeZone = string.IsNullOrEmpty(_configuration.TimeZone) ? TimeZoneInfo.Local.Id : _configuration.TimeZone,
                    PipelineThreading = (PipelineThreading)Enum.Parse(typeof(PipelineThreading), _configuration.PipelineThreading, true),
                    Parallel = _configuration.Parallel,
                    Runner = _runner
                };

                //shared across the process
                _process.Connections.AddRange(_configuration.Connections);
                _process.OutputConnection = _process.Connections.Output().Connection;

                _process.DataSets = GetDataSets(_configuration);
                _process.Scripts = new ScriptReader(_configuration.Scripts, _logger).Read();
                _process.Actions = new ActionReader(_process).Read(_configuration.Actions);
                _process.Templates = new TemplateReader(_process, _configuration.Templates).Read();
                _process.SearchTypes = new SearchTypeReader(_configuration.SearchTypes).Read();
                new MapLoader(_process, _configuration.Maps).Load();

                //these depend on the shared process properties
                new EntitiesLoader(_process, _configuration.Entities).Load();
                new OperationsLoader(_process, _configuration.Entities).Load();

                _process.Relationships = new RelationshipsReader(_process, _configuration.Relationships).Read();
                new ProcessOperationsLoader(_process, _configuration.CalculatedFields).Load();
                new EntityRelationshipLoader(_process).Load();

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

        }

    }
}
