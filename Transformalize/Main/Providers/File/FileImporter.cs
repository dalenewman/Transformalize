using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Transformalize.Configuration;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.File {
    public class FileImporter {

        public void ImportScaler(FileInfo fileInfo, TflConnection output, string processName = null, string entityName = null) {
            ImportScaler(fileInfo, new FileInspectionRequest(), output, processName, entityName);
        }

        public void ImportScaler(FileInfo fileInfo, FileInspectionRequest request, TflConnection output, string processName = null, string entityName = null) {
            var fileInformation = FileInformationFactory.Create(fileInfo, request);
            var configuration = BuildProcess(fileInformation, request, output, processName, entityName);
            var process = ProcessFactory.CreateSingle(configuration);
            if (process.Connections["output"].Type != ProviderType.Internal) {
                process.Options.Mode = "init";
                process.Mode = "init";
                process.ExecuteScaler();
            }

            ProcessFactory.CreateSingle(configuration, new Options()).ExecuteScaler();
        }

        public FileImportResult Import(FileInfo fileInfo, TflConnection output, string processName = null, string entityName = null) {
            return Import(fileInfo, new FileInspectionRequest(), output, processName, entityName);
        }

        public FileImportResult Import(FileInfo fileInfo, FileInspectionRequest request, TflConnection output, string processName = null, string entityName = null) {
            var fileInformation = FileInformationFactory.Create(fileInfo, request);
            var configuration = BuildProcess(fileInformation, request, output, processName, entityName);
            var process = ProcessFactory.CreateSingle(configuration);

            if (process.Connections["output"].Type != ProviderType.Internal) {
                process.Options.Mode = "init";
                process.Mode = "init";
                process.ExecuteScaler();
            }

            return new FileImportResult {
                Information = fileInformation,
                Rows = ProcessFactory.CreateSingle(configuration).Execute()
            };
        }

        private static TflProcess BuildProcess(FileInformation fileInformation, FileInspectionRequest request, TflConnection output, string processName = null, string entityName = null) {

            if (String.IsNullOrEmpty(processName)) {
                processName = Common.CleanIdentifier(Path.GetFileNameWithoutExtension(fileInformation.FileInfo.Name));
            }

            if (String.IsNullOrEmpty(entityName)) {
                entityName = "TflAuto" + processName.GetHashCode().ToString(CultureInfo.InvariantCulture).Replace("-", "0").PadRight(13, '0');
            }

            var root = new TflRoot(string.Format(@"<tfl><processes><add name='{0}'><connections><add name='input' provider='internal' /></connections></add></processes></tfl>", entityName), null);

            var process = root.GetDefaultOf<TflProcess>(p => {
                p.Name = entityName;
                p.Star = processName;
                p.StarEnabled = false;
            });

            process.Connections.Add(process.GetDefaultOf<TflConnection>(c => {
                c.Name = "input";
                c.Provider = "file";
                c.File = fileInformation.FileInfo.FullName;
                c.Delimiter = fileInformation.Delimiter == default(char) ? "|" : fileInformation.Delimiter.ToString(CultureInfo.InvariantCulture);
                c.Start = fileInformation.FirstRowIsHeader ? 2 : 1;
            }));

            process.Connections.Add(output);

            process.Entities.Add(process.GetDefaultOf<TflEntity>(e => {
                e.Name = entityName;
                e.PrependProcessNameToOutputName = false;
                e.DetectChanges = false;
            }));

            var fields = new FieldInspector().Inspect(fileInformation, request);

            foreach (var fd in fields) {
                if (fd.Type.Equals("string")) {
                    TflLogger.Info(processName, entityName, "Using {0} character string for {1}.", fd.Length, fd.Name);
                } else {
                    TflLogger.Info(processName, entityName, "Using {0} for {1}.", fd.Type, fd.Name);
                }

                var field = fd;
                process.Entities[0].Fields.Add(process.GetDefaultOf<TflField>(f => {
                    f.Name = field.Name;
                    f.Length = field.Length;
                    f.Type = field.Type;
                    f.QuotedWith = field.QuotedWith;
                }));
            }

            return process;
        }

    }
}