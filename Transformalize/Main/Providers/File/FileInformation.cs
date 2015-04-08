using System.Globalization;
using System.IO;
using System.Linq;

namespace Transformalize.Main.Providers.File {

    public class FileInformation {

        private bool _firstRowIsHeader = true;
        private Fields _fields = new Fields();

        //properties
        public FileInfo FileInfo { get; private set; }

        public Fields Fields {
            get { return _fields; }
            set { _fields = value; }
        }

        public char Delimiter { get; set; }

        public bool FirstRowIsHeader {
            get { return _firstRowIsHeader; }
            set { _firstRowIsHeader = value; }
        }

        //constructors
        public FileInformation(FileInfo fileInfo) {
            FileInfo = fileInfo;
        }

        //methods
        public int ColumnCount() {
            return Fields.Count();
        }

    }
}