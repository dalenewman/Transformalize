using System;
using System.Globalization;
using System.IO;
using Transformalize.Configuration;
using Transformalize.Configuration.Builders;
using Transformalize.Libs.NLog;

namespace Transformalize.Main.Providers.File {
    public class FileImporter {

        private readonly Logger _log = LogManager.GetLogger("tfl");

        public void ImportScaler(FileInfo fileInfo, ConnectionConfigurationElement output, string processName = null, string entityName = null) {
            ImportScaler(fileInfo, new FileInspectionRequest(), output, processName, entityName);
        }

        public void ImportScaler(FileInfo fileInfo, FileInspectionRequest request, ConnectionConfigurationElement output, string processName = null, string entityName = null) {
            var fileInformation = FileInformationFactory.Create(fileInfo, request);
            var process = BuildProcess(fileInformation, request, output, processName, entityName);
            ProcessFactory.Create(process, new Options { Mode = "init" })[0].ExecuteScaler();
            ProcessFactory.Create(process)[0].ExecuteScaler();
        }

        public FileImportResult Import(FileInfo fileInfo, ConnectionConfigurationElement output, string processName = null, string entityName = null) {
            return Import(fileInfo, new FileInspectionRequest(), output, processName, entityName);
        }

        public FileImportResult Import(FileInfo fileInfo, FileInspectionRequest request, ConnectionConfigurationElement output, string processName = null, string entityName = null) {
            var fileInformation = FileInformationFactory.Create(fileInfo, request);
            var process = BuildProcess(fileInformation, request, output, processName, entityName);
            ProcessFactory.CreateSingle(process, new Options { Mode = "init" }).ExecuteScaler();
            return new FileImportResult {
                Information = fileInformation,
                Rows = ProcessFactory.Create(process)[0].Execute()
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

            foreach (var field in fields) {
                if (field.Type.Equals("string")) {
                    _log.Info("Using {0} character string for {1}.", field.Length, field.Name);
                } else {
                    _log.Info("Using {0} for {1}.", field.Type, field.Name);
                }

                builder
                    .Field(field.Name)
                    .Length(field.Length)
                    .Type(field.Type)
                    .QuotedWith(field.QuotedWith);

            }

            var process = builder.Process();
            _log.Debug(process.Serialize().Replace(Environment.NewLine, string.Empty));
            return process;
        }

    }
}