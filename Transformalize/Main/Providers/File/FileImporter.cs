using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Configuration.Builders;
using Transformalize.Libs.NLog;

namespace Transformalize.Main.Providers.File {
    public class FileImporter {
        private readonly Logger _log = LogManager.GetLogger("tfl");

        public FileImportResult Import(FileInfo fileInfo, FileInspectionRequest request, ConnectionConfigurationElement output) {

            var fileInformation = FileInformationFactory.Create(fileInfo, request);

            var entityName = fileInformation.Identifier("TflAuto");

            var builder = new ProcessBuilder(entityName)
                .Star(fileInformation.ProcessName)
                .Connection("input")
                    .Provider("file")
                    .File(fileInformation.FileInfo.FullName)
                    .Delimiter(fileInformation.Delimiter.ToString(CultureInfo.InvariantCulture))
                    .Start(fileInformation.FirstRowIsHeader ? 2 : 1)
                .Connection("output")
                    .Element(output)
                .Entity(entityName)
                    .PrependProcessNameToOutputName(false)
                    .DetectChanges(false);

            var fields = new FieldInspector().Inspect(fileInformation, request);

            foreach (var fileField in fields) {
                if (fileField.Type.Equals("string")) {
                    _log.Info("Using {0} character string for {1}.", fileField.Length, fileField.Name);
                } else {
                    _log.Info("Using {0} for {1}.", fileField.Type, fileField.Name);
                }

                builder
                    .Field(fileField.Name)
                    .Length(fileField.Length)
                    .Type(fileField.Type)
                    .QuotedWith(fileField.QuoteString());
            }

            _log.Debug(builder.Process().Serialize().Replace(Environment.NewLine, string.Empty));

            ProcessFactory.Create(builder.Process(), new Options { Mode = "init" })[0].Execute();
            return new FileImportResult {
                Fields = fields,
                FileInformation = fileInformation,
                Rows = ProcessFactory.Create(builder.Process())[0].Execute()[entityName]
            };
        }

    }
}