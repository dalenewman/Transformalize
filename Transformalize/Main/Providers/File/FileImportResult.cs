using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main.Providers.File {
    public class FileImportResult {
        public FileInformation FileInformation { get; set; }
        public IEnumerable<FileField> Fields { get; set; }
        public IEnumerable<Row> Rows { get; set; }
    }
}