using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.File {

    public class FieldInspector {

        public Fields Inspect(string file) {
            return Inspect(FileInformationFactory.Create(file), new FileInspectionRequest());
        }

        public Fields Inspect(FileInformation fileInformation) {
            return Inspect(fileInformation, new FileInspectionRequest());
        }

        public Fields Inspect(FileInformation fileInformation, FileInspectionRequest request) {

            var root = new TflRoot(string.Format(@"<tfl><processes><add name='{0}'><connections><add name='input' provider='internal' /></connections></add></processes></tfl>", fileInformation.ProcessName), null);
            var process = root.GetDefaultOf<TflProcess>(p => {
                p.Name = fileInformation.ProcessName;
                p.StarEnabled = false;
            });

            process.Connections.Add(process.GetDefaultOf<TflConnection>(c => {
                c.Name = "input";
                c.Provider = "file";
                c.File = fileInformation.FileInfo.FullName;
                c.Delimiter = fileInformation.Delimiter == default(char) ? '|' : fileInformation.Delimiter;
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

            var runner = ProcessFactory.CreateSingle(process);
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