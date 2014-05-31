using System;
using System.Collections.Generic;
using System.IO;
using Transformalize.Libs.ExcelDataReader;

namespace Transformalize.Main.Providers.File {

    public class ExcelInformationReader {
        private readonly FileInspectionRequest _request;

        public ExcelInformationReader(FileInspectionRequest request) {
            _request = request;
        }

        public FileInformation Read(FileInfo fileInfo) {

            var fileInformation = new FileInformation(fileInfo);
            var columnNames = new List<string>();

            var stream = System.IO.File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var isXml = fileInfo.Extension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase);

            var excelReader = isXml ? ExcelReaderFactory.CreateOpenXmlReader(stream) : ExcelReaderFactory.CreateBinaryReader(stream);
            excelReader.Read();
            for (var i = 0; i < excelReader.FieldCount; i++) {
                var name = excelReader.GetString(i);
                if (name != null)
                    columnNames.Add(name);
            }

            excelReader.Close();
            foreach (var value in columnNames) {
                fileInformation.Fields.Add(new FileField(value, _request.DefaultType, _request.DefaultLength));
            }

            return fileInformation;
        }
    }
}