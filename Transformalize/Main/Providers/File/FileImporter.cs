using System;
using System.Globalization;
using System.IO;
using Transformalize.Configuration;
using Transformalize.Configuration.Builders;
using Transformalize.Libs.NLog;

namespace Transformalize.Main.Providers.File {
    public class FileImporter {

        private readonly Logger _log = LogManager.GetLogger("tfl");

        public void ImportScaler(FileInfo fileInfo, FileInspectionRequest request, ConnectionConfigurationElement output, string processName = null, string entityName = null) {
            var fileInformation = FileInformationFactory.Create(fileInfo, request);
            var process = BuildProcess(fileInformation, request, output, processName, entityName);
            ProcessFactory.Create(process, new Options { Mode = "init" })[0].ExecuteScaler();
            ProcessFactory.Create(process)[0].ExecuteScaler();
        }

        public FileImportResult Import(FileInfo fileInfo, FileInspectionRequest request, ConnectionConfigurationElement output, string processName = null, string entityName = null) {
            var fileInformation = FileInformationFactory.Create(fileInfo, request);
            var process = BuildProcess(fileInformation, request, output, processName, entityName);
            return new FileImportResult {
                Information = fileInformation,
                Rows = ProcessFactory.Create(process)[0].ExecuteSingle()
            };
        }

        private ProcessConfigurationElement BuildProcess(FileInformation fileInformation, FileInspectionRequest request, ConnectionConfigurationElement output, string processName = null, string entityName = null) {

            if (String.IsNullOrEmpty(processName)) {
                processName = Common.CleanIdentifier(Path.GetFileNameWithoutExtension(fileInformation.FileInfo.Name));
            }

            if (String.IsNullOrEmpty(entityName)) {
                entityName = "TflAuto" + processName.GetHashCode().ToString(CultureInfo.InvariantCulture).Replace("-", "0").PadRight(13, '0');
            }

            var builder = new ProcessBuilder(entityName)
                .Star(processName)
                .Connection("input")
                    .Provider("file")
                    .File(fileInformation.FileInfo.FullName)
                    .Delimiter(fileInformation.Delimiter == default(char) ? "|" : fileInformation.Delimiter.ToString(CultureInfo.InvariantCulture))
                    .Start(fileInformation.FirstRowIsHeader ? 2 : 1)
                .Connection(output)
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

            var process = builder.Process();
            _log.Debug(process.Serialize().Replace(Environment.NewLine, string.Empty));
            return process;
        }

    }
}