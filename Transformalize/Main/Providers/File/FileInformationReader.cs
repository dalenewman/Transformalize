using System.IO;

namespace Transformalize.Main.Providers.File {

    public class FileInformationReader {
        private readonly FileInspectionRequest _request;

        public FileInformationReader(FileInspectionRequest request)
        {
            _request = request;
        }

        public FileInformation Read(FileInfo fileInfo) {

            var lines = new Lines(fileInfo, _request);
            var bestDelimiter = lines.FindDelimiter();

            return new FileInformation(fileInfo) {
                Delimiter = bestDelimiter,
                Fields = lines.InitialFieldTypes()
            };
        }
    }
}

