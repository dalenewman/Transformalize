using System.Collections.Generic;
using System.IO;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Operations.Extract
{
    public class FileContentsExtract : AbstractOperation {

        private readonly FileInfo _fileInfo;
        private readonly string _output;

        public FileContentsExtract(AbstractConnection fileConnection, Entity entity) {
            _fileInfo = new FileInfo(fileConnection.File);
            _output = entity.Fields.First().Alias;
            if (!_fileInfo.Exists) {
                throw new TransformalizeException(ProcessName, entity.Alias, "File {0} does not exist.", fileConnection.Name);
            }
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            var row = new Row();
            row[_output] = System.IO.File.ReadAllText(_fileInfo.FullName);
            return new[] { row };
        }
    }
}