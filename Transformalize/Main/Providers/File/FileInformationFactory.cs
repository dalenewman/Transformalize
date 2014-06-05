using System;
using System.IO;
using System.Linq;

namespace Transformalize.Main.Providers.File {
    public static class FileInformationFactory {

        public static FileInformation Create(string file) {
            return Create(new FileInfo(file), new FileInspectionRequest());
        }

        public static FileInformation Create(string file, FileInspectionRequest request) {
            return Create(new FileInfo(file), new FileInspectionRequest());
        }

        public static FileInformation Create(FileInfo fileInfo, FileInspectionRequest request) {
            var ext = fileInfo.Extension.ToLower();

            var fileInformation = ext.StartsWith(".xls", StringComparison.OrdinalIgnoreCase) ?
                new ExcelInformationReader(request).Read(fileInfo) :
                new FileInformationReader(request).Read(fileInfo);

            var validator = new ColumnNameValidator(fileInformation.Fields.Select(f => f.Name));
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