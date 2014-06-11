using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Transformalize.Main.Providers.File {

    public class FileInformation {
        private string _processName;
        private string _entityName;

        private bool _firstRowIsHeader = true;
        private Fields _fields = new Fields();

        //properties
        public FileInfo FileInfo { get; private set; }

        public Fields Fields {
            get { return _fields; }
            set { _fields = value; }
        }

        public string ProcessName {
            get { return _processName ?? (_processName = Common.CleanIdentifier(Path.GetFileNameWithoutExtension(FileInfo.Name))); }
        }

        public string EntityName {
            get { return _entityName ?? (_entityName = "TflAuto" + ProcessName.GetHashCode().ToString(CultureInfo.InvariantCulture).Replace("-", "0").PadLeft(13, '0')); }
        }

        public char Delimiter { get; set; }

        public bool FirstRowIsHeader {
            get { return _firstRowIsHeader; }
            set { _firstRowIsHeader = value; }
        }

        //constructors
        public FileInformation(FileInfo fileInfo, string processName = null, string entityName = null) {
            _processName = processName;
            _entityName = entityName;
            FileInfo = fileInfo;
        }

        //methods
        public int ColumnCount() {
            return Fields.Count();
        }

    }
}