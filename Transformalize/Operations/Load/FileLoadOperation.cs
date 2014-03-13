using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Rhino.Etl.Core.Files;
using Transformalize.Libs.FileHelpers.RunTime;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Operations.Load
{
    public class FileLoadOperation : AbstractOperation {
        private readonly FileInfo _fileInfo;
        private readonly Type _type;

        public FileLoadOperation(AbstractConnection connection, Entity entity) {
            _fileInfo = new FileInfo(connection.File);

            if (_fileInfo.DirectoryName != null && !Directory.Exists(_fileInfo.DirectoryName)) {
                Info("Creating Output Folder(s).");
                Directory.CreateDirectory(_fileInfo.DirectoryName);
            }

            if (!_fileInfo.Exists) {
                Warn("Output file already exists.  Deleting...");
                _fileInfo.Delete();
            }

            var builder = new DelimitedClassBuilder("Tfl" + entity.OutputName()) { IgnoreEmptyLines = true, Delimiter = connection.Delimiter, IgnoreFirstLines = 0 };
            foreach (var pair in entity.Fields.Where(f => f.Value.FileOutput)) {
                builder.AddField(pair.Value.Alias, pair.Value.SystemType);
            }
            foreach (var pair in entity.CalculatedFields.Where(f => f.Value.FileOutput)) {
                builder.AddField(pair.Value.Alias, pair.Value.SystemType);
            }
            _type = builder.CreateRecordClass();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            var engine = new FluentFile(_type);
            if (!string.IsNullOrEmpty(string.Empty)) {
                engine.HeaderText = string.Empty;
            }

            using (var file = engine.To(_fileInfo.FullName)) {
                foreach (var row in rows) {
                    var record = row.ToObject(_type);
                    file.Write(record);
                }
            }
            yield break;
        }
    }
}