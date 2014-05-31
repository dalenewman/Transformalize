using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Transformalize.Main.Providers.File {

    public class FileInformation {

        private bool _firstRowIsHeader = true;
        private FileInfo _fileInfo;
        private List<FileField> _fields = new List<FileField>();

        //properties
        public FileInfo FileInfo {
            get { return _fileInfo; }
            set { _fileInfo = value; }
        }

        public List<FileField> Fields {
            get { return _fields; }
            set { _fields = value; }
        }

        public string ProcessName { get { return Common.CleanIdentifier(Path.GetFileNameWithoutExtension(_fileInfo.Name)); } }
        public char Delimiter { get; set;}
        public bool FirstRowIsHeader {
            get { return _firstRowIsHeader; }
            set { _firstRowIsHeader = value; }
        }

        //constructors
        public FileInformation(FileInfo fileInfo) {
            _fileInfo = fileInfo;
        }

        //methods
        public int ColumnCount() {
            return Fields.Count();
        }

        public string Identifier(string prefix = "") {
            return prefix + ProcessName.GetHashCode().ToString(CultureInfo.InvariantCulture).Replace("-", "0").PadRight(13, '0');
        }

    }
}