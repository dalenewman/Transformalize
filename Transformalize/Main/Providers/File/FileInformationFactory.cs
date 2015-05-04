using System;
using System.Linq;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.File {

    public static class FileInformationFactory {

        public static FileInformation Create(string file, ILogger logger, string processName = null, string entityName = null) {
            return Create(new FileInspectionRequest(file), logger, processName, entityName);
        }

        public static FileInformation Create(FileInspectionRequest request, ILogger logger, string processName = null, string entityName = null) {

            var ext = request.FileInfo.Extension.ToLower();
            var isExcel = ext.StartsWith(".xls", StringComparison.OrdinalIgnoreCase);

            var fileInformation = isExcel ?
                new ExcelInformationReader(request).Read(request.FileInfo) :
                new FileInformationReader(request, logger).Read(request.FileInfo);

            var validator = new ColumnNameValidator(fileInformation.Fields.Select(f => f.Name).ToArray());
            if (validator.Valid())
                return fileInformation;

            fileInformation.FirstRowIsHeader = false;
            for (var i = 0; i < fileInformation.Fields.Count(); i++) {
                fileInformation.Fields[i].Name = ColumnNameGenerator.CreateDefaultColumnName(i);
            }

            return fileInformation;
        }
    }
}