using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main.Providers.File
{
    public class FileImportResult {
        public FileInformation Information { get; set; }
        public IEnumerable<Row> Rows { get; set; }
    }
}