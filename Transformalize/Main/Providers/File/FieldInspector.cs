using System.Globalization;
using System.Linq;
using Transformalize.Configuration.Builders;
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

            var builder = new ProcessBuilder(fileInformation.ProcessName)
                .StarEnabled(false)
                .Connection("input")
                    .Provider("file")
                    .File(fileInformation.FileInfo.FullName)
                    .Delimiter(fileInformation.Delimiter == default(char) ? "|" : fileInformation.Delimiter.ToString(CultureInfo.InvariantCulture))
                    .Start(fileInformation.FirstRowIsHeader ? 2 : 1)
                .Connection("output")
                    .Provider("internal")
                .Entity("Data").DetectChanges(false)
                    .Sample(request.Sample);

            foreach (var field in fileInformation.Fields) {
                builder
                    .Field(field.Name)
                    .Length(field.Length)
                    .Type(field.Type)
                    .QuotedWith(field.QuotedWith);
            }

            foreach (var dataType in request.DataTypes) {
                foreach (var field in fileInformation.Fields) {
                    var result = IsDataTypeField(field.Name, dataType);
                    builder.CalculatedField(result).Bool()
                        .Transform("typeconversion")
                            .Type(dataType)
                            .ResultField(result)
                            .MessageField(string.Empty)
                            .Parameter(field.Name)
                            .IgnoreEmpty(request.IgnoreEmpty);
                }
            }

            foreach (var field in fileInformation.Fields) {
                var result = LengthField(field.Name);
                builder.CalculatedField(result).Int32()
                    .Transform("length")
                    .Parameter(field.Name);
            }

            var runner = ProcessFactory.CreateSingle(builder.Process(), new Options() { Top = request.Top });
            var results = runner.Execute().ToList();

            if (results.Count <= 0) {
                TflLogger.Warn(string.Empty,string.Empty,"Nothing imported from in {0}!", fileInformation.FileInfo.Name);
                return fileInformation.Fields;
            }

            foreach (var field in fileInformation.Fields) {
                if (!results.All(row => row[field.Name].Equals(string.Empty))) {
                    foreach (var dataType in request.DataTypes) {
                        var result = IsDataTypeField(field.Name, dataType);
                        if (!results.All(row => row[result].Equals(true)))
                            continue;
                        field.Type = dataType;
                        field.Length = string.Empty;
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