using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.File {

    public class FieldInspector {

        public Fields Inspect(string file) {
            return Inspect(FileInformationFactory.Create(file), new FileInspectionRequest(file));
        }

        public Fields Inspect(FileInformation fileInformation) {
            return Inspect(fileInformation, new FileInspectionRequest(fileInformation.FileInfo.Name));
        }

        public Fields Inspect(FileInformation fileInformation, FileInspectionRequest request) {

            var process = new TflProcess() {
                Name = request.ProcessName,
                StarEnabled = false
            };

            process.Connections.Add(new TflConnection {
                Name = "input",
                Provider = "file",
                File = fileInformation.FileInfo.FullName,
                Delimiter = fileInformation.Delimiter == default(char) ? "|" : fileInformation.Delimiter.ToString(CultureInfo.InvariantCulture),
                Start = fileInformation.FirstRowIsHeader ? 2 : 1,
            });

            process.Connections.Add(new TflConnection {
                Name = "output",
                Provider = "internal"
            });

            process.Entities.Add(new TflEntity {
                Name = "Data",
                DetectChanges = false,
                Sample = System.Convert.ToInt32(request.Sample),
                Fields = new List<TflField>(),
                CalculatedFields = new List<TflField>()
            });

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

            var runner = ProcessFactory.CreateSingle(new TflRoot(process).Processes[0]);
            var results = runner.Execute().ToList();

            if (results.Count <= 0) {
                TflLogger.Warn(string.Empty, string.Empty, "Nothing imported from in {0}!", fileInformation.FileInfo.Name);
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