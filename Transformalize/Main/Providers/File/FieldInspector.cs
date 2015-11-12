using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cfg.Net;
using Cfg.Net.Ext;
using Transformalize.Configuration;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.File {

    public class FieldInspector {

        private readonly ILogger _logger;

        public FieldInspector(ILogger logger) {
            _logger = logger;
        }

        public Fields Inspect(string file) {
            return Inspect(FileInformationFactory.Create(file, _logger), new FileInspectionRequest(file));
        }

        public Fields Inspect(FileInformation fileInformation) {
            return Inspect(fileInformation, new FileInspectionRequest(fileInformation.FileInfo.Name));
        }

        public Fields Inspect(FileInformation fileInformation, FileInspectionRequest request) {

            var process = new TflProcess{
                Name = request.ProcessName,
                StarEnabled = false,
                ViewEnabled = false,
                PipelineThreading = "MultiThreaded"
            }.WithDefaults();

            process.Connections = new List<TflConnection> {
                new TflConnection {
                    Name = "input",
                    Provider = "file",
                    File = fileInformation.FileInfo.FullName,
                    Delimiter = fileInformation.Delimiter == default(char)
                        ? "|"
                        : fileInformation.Delimiter.ToString(CultureInfo.InvariantCulture),
                    Start = fileInformation.FirstRowIsHeader ? 2 : 1
                }.WithDefaults(),
                new TflConnection {
                    Name = "output",
                    Provider = "internal"
                }.WithDefaults()
            };

            process.Entities.Add(new TflEntity {
                Name = request.EntityName,
                PrependProcessNameToOutputName = false,
                DetectChanges = false,
                Sample = System.Convert.ToInt32(request.Sample)
            }.WithDefaults());

            foreach (var fd in fileInformation.Fields) {
                var field = fd;
                process.Entities[0].Fields.Add(new TflField{
                    Name = field.Name,
                    Length = field.Length,
                    Type = field.Type,
                    QuotedWith = field.QuotedWith
                }.WithDefaults());
            }

            for (var i = 0; i < request.DataTypes.Count; i++) {
                var dataType = request.DataTypes[i];
                foreach (var field in fileInformation.Fields) {
                    var result = IsDataTypeField(field.Name, dataType);
                    process.Entities[0].CalculatedFields.Add(
                        new TflField{
                            Name = result,
                            Type = "bool",
                            Input = false,
                            Transforms = new List<TflTransform> {
                                new TflTransform {
                                    Method = "typeconversion",
                                    Type = dataType,
                                    Parameter = field.Name,
                                    IgnoreEmpty = request.IgnoreEmpty
                                }.WithDefaults()
                            }
                        }.WithDefaults()
                    );
                }
            }

            foreach (var field in fileInformation.Fields) {
                var result = LengthField(field.Name);
                process.Entities[0].CalculatedFields.Add(
                    new TflField {
                        Name = result,
                        Type = "int32",
                        Transforms = new List<TflTransform> {
                            new TflTransform {
                                Method = "length",
                                Parameter = field.Name
                            }.WithDefaults()
                        }
                    }.WithDefaults()
                );
            }

            var runner = ProcessFactory.CreateSingle(new TflRoot(process).Processes[0], _logger);
            var results = runner.Execute().ToList();

            if (results.Count <= 0) {
                _logger.Warn("Nothing imported from in {0}!", fileInformation.FileInfo.Name);
                return fileInformation.Fields;
            }

            foreach (var field in fileInformation.Fields) {
                if (!results.All(row => row[field.Name].Equals(string.Empty))) {
                    foreach (var dataType in request.DataTypes) {
                        var result = IsDataTypeField(field.Name, dataType);
                        if (!results.All(row => row[result].Equals(true)))
                            continue;
                        field.Type = dataType;
                        field.Length = request.MinLength.ToString(CultureInfo.InvariantCulture);
                        break;
                    }
                }
                if (!field.Type.Equals("string"))
                    continue;

                var length = results.Max(row => (int)row[LengthField(field.Name)]) + 1;
                if (request.MaxLength > 0 && length > request.MaxLength) {
                    length = request.MaxLength;
                }
                if (request.MinLength > 0 && length < request.MinLength) {
                    length = request.MinLength;
                }
                field.Length = length.ToString(CultureInfo.InvariantCulture);
            }
            return fileInformation.Fields;
        }

        private static string IsDataTypeField(string name, string dataType) {
            return name + "Is" + char.ToUpper(dataType[0]) + dataType.Substring(1);
        }

        private static string LengthField(string name) {
            return name + "Length";
        }

    }
}