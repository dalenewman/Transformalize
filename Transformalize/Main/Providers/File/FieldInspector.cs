using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

            var process = new TflProcess().GetDefaultOf<TflProcess>(p => {
                p.Name = request.ProcessName;
                p.StarEnabled = false;
            });

            process.Connections.Add(process.GetDefaultOf<TflConnection>(c => {
                c.Name = "input";
                c.Provider = "file";
                c.File = fileInformation.FileInfo.FullName;
                c.Delimiter = fileInformation.Delimiter == default(char)
                    ? "|"
                    : fileInformation.Delimiter.ToString(CultureInfo.InvariantCulture);
                c.Start = fileInformation.FirstRowIsHeader ? 2 : 1;
            }));

            process.Connections.Add(process.GetDefaultOf<TflConnection>(c => {
                c.Name = "output";
                c.Provider = "internal";
            }));

            process.Entities.Add(process.GetDefaultOf<TflEntity>(e => {
                e.Name = "Data";
                e.DetectChanges = false;
                e.Sample = System.Convert.ToInt32(request.Sample);
                e.Fields = new List<TflField>();
                e.CalculatedFields = new List<TflField>();
            }));

            foreach (var fd in fileInformation.Fields) {
                var field = fd;
                process.Entities[0].Fields.Add(process.GetDefaultOf<TflField>(f => {
                    f.Name = field.Name;
                    f.Length = field.Length;
                    f.Type = field.Type;
                    f.QuotedWith = field.QuotedWith;
                }));
            }

            for (var i = 0; i < request.DataTypes.Count; i++) {
                var dataType = request.DataTypes[i];
                foreach (var field in fileInformation.Fields) {
                    var result = IsDataTypeField(field.Name, dataType);
                    process.Entities[0].CalculatedFields.Add(
                        process.GetDefaultOf<TflField>(f => {
                            f.Name = result;
                            f.Type = "bool";
                            f.Input = false;
                            f.Transforms = new List<TflTransform> {
                                f.GetDefaultOf<TflTransform>(t => {
                                    t.Method = "typeconversion";
                                    t.Type = dataType;
                                    t.Parameter = field.Name;
                                    t.IgnoreEmpty = request.IgnoreEmpty;
                                })
                            };
                        })
                    );
                }
            }

            foreach (var field in fileInformation.Fields) {
                var result = LengthField(field.Name);
                process.Entities[0].CalculatedFields.Add(
                    process.GetDefaultOf<TflField>(f => {
                        f.Name = result;
                        f.Type = "int32";
                        f.Transforms = new List<TflTransform> {
                            f.GetDefaultOf<TflTransform>(t => {
                                t.Method = "length";
                                t.Parameter = field.Name;
                            })
                        };
                    })
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