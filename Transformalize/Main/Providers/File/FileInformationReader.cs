using System.IO;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.File {

    public class FileInformationReader {

        private readonly FileInspectionRequest _request;
        private readonly ILogger _logger;

        public FileInformationReader(FileInspectionRequest request, ILogger logger)
        {
            _request = request;
            _logger = logger;
        }

        public FileInformation Read(FileInfo fileInfo) {

            var lines = new Lines(fileInfo, _request, _logger);
            var bestDelimiter = lines.FindDelimiter();

            return new FileInformation(fileInfo) {
                Delimiter = bestDelimiter,
                Fields = lines.InitialFieldTypes()
            };
        }
    }
}

