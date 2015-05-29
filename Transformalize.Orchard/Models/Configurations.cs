using System.Collections.Generic;
using Transformalize.Orchard.Services;

namespace Transformalize.Orchard.Models {
    public class Configurations {
        private readonly IFileService _fileService;
        private string _inputFileName = string.Empty;
        private string _outputFileName = string.Empty;
        private int _inputFileId;
        private int _outputFileId;

        public IEnumerable<ConfigurationPart> ConfigurationParts { get; set; }

        public int InputFileId {
            get { return _inputFileId; }
            set {
                if (value <= 0 || _inputFileId.Equals(value)) {
                    return;
                }
                _inputFileId = value;
                _inputFileName = _fileService.Get(value).FileName();
            }
        }

        public int OutputFileId {
            get { return _outputFileId; }
            set {
                if (value <= 0 || _outputFileId.Equals(value)) {
                    return;
                }
                _outputFileId = value;
                _outputFileName = _fileService.Get(value).FileName();
            }
        }

        public string InputFileName {
            get { return _inputFileName; }
        }

        public string OutputFileName {
            get { return _outputFileName; }
        }

        public string Mode { get; set; }
        public int CurrentId { get; set; }
        public bool Edit { get; set; }
        public bool Enqueue { get; set; }

        public Configurations(IFileService fileService) {
            _fileService = fileService;
        }
    }
}