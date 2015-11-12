using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Transformalize.Configuration;
using Cfg.Net;
using Cfg.Net.Ext;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.File {

    public class FileImporter {

        private readonly ILogger _logger;

        public FileImporter(ILogger logger) {
            _logger = logger;
        }

        /// <summary>
        /// Imports a file to an output with default file inspection settings
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public long ImportScaler(string fileName, TflConnection output) {
            return ImportScaler(new FileInspectionRequest(fileName), output);
        }

        /// <summary>
        /// Imports a file to an output with provided file inpections settings
        /// </summary>
        /// <param name="request"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public long ImportScaler(FileInspectionRequest request, TflConnection output) {
            return Import(request, output).RowCount;
        }

        public FileImportResult Import(string fileName, TflConnection output) {
            return Import(new FileInspectionRequest(fileName), output);
        }

        public FileImportResult Import(FileInspectionRequest request, TflConnection output) {

            var fileInformation = FileInformationFactory.Create(request, _logger);

            var cfg = BuildProcess(fileInformation, request, output, _logger);

            if (cfg.Connections.First(c => c.Name == "output").Provider == "internal") {
                // nothing to init, so just run in default mode
                return new FileImportResult {
                    Information = fileInformation,
                    Rows = ProcessFactory.CreateSingle(cfg, _logger).Execute()
                };
            }

            // first run in init mode
            cfg.Mode = "init";
            var process = ProcessFactory.CreateSingle(cfg, _logger, new Options { Mode = "init" });
            process.ExecuteScaler();

            // now run in default mode
            cfg.Mode = "default";
            process = ProcessFactory.CreateSingle(cfg, _logger, new Options() { Mode = "default" });
            return new FileImportResult {
                Information = fileInformation,
                Rows = process.Execute(),
                RowCount = process.Entities[0].Inserts
            };
        }

        private static TflProcess BuildProcess(
            FileInformation fileInformation,
            FileInspectionRequest request,
            TflConnection output,
            ILogger logger) {

            var process = new TflProcess{
                Name = request.EntityName,
                Star = request.ProcessName,
                StarEnabled = false,
                ViewEnabled = false,
                PipelineThreading = "MultiThreaded",
                Connections = new List<TflConnection> {
                    new TflConnection{
                        Name = "input",
                        Provider = "file",
                        File = fileInformation.FileInfo.FullName,
                        Delimiter = fileInformation.Delimiter == default(char)
                            ? "|"
                            : fileInformation.Delimiter.ToString(CultureInfo.InvariantCulture),
                        Start = fileInformation.FirstRowIsHeader ? 2 : 1
                    }.WithDefaults(),
                    output
                },
                Entities = new List<TflEntity> {
                    new TflEntity{
                        Name = request.EntityName,
                        Connection = "input",
                        PrependProcessNameToOutputName = false,
                        DetectChanges = false,
                        Fields = GetFields(new FieldInspector(logger).Inspect(fileInformation, request), logger, request.EntityName)
                    }.WithDefaults()
                }
            }.WithDefaults();

            return process;
        }

        private static List<TflField> GetFields(IEnumerable<Field> fieldDefinitions, ILogger logger, string entityName) {
            var fields = new List<TflField>();
            foreach (var fd in fieldDefinitions) {
                if (fd.Type.Equals("string")) {
                    logger.EntityInfo(entityName, "Using {0} character string for {1}.", fd.Length, fd.Name);
                } else {
                    logger.EntityInfo(entityName, "Using {0} for {1}.", fd.Type, fd.Name);
                }

                var field = fd;
                fields.Add(new TflField{
                    Name = field.Name,
                    Length = field.Length,
                    Type = field.Type,
                    QuotedWith = field.QuotedWith
                }.WithDefaults());
            }
            return fields;
        }
    }
}