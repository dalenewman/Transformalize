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

namespace Transformalize.Operations.Load {
    public class FileLoadOperation : AbstractOperation {
        private readonly AbstractConnection _connection;
        private readonly Entity _entity;
        protected FileInfo FileInfo { get; private set; }
        protected Type Type { get; set; }
        protected List<string> Headers { get; set; }
        protected string HeaderText { get; set; }
        protected string FooterText { get; set; }

        public FileLoadOperation(AbstractConnection connection, Entity entity) {
            FileInfo = new FileInfo(connection.File);
            Headers = new List<string>();
            HeaderText = string.Empty;
            FooterText = string.Empty;
            _connection = connection;
            _entity = entity;

            if (FileInfo.DirectoryName != null && !Directory.Exists(FileInfo.DirectoryName)) {
                Info("Creating Output Folder(s).");
                Directory.CreateDirectory(FileInfo.DirectoryName);
            }

            if (FileInfo.Exists)
                return;
            Warn("Output file already exists.  Deleting...");

            FileInfo.Delete();
        }

        protected virtual void PrepareHeader(Entity entity) {
            HeaderText = string.Empty;
            foreach (var pair in entity.Fields.Where(f => f.Value.FileOutput)) {
                Headers.Add(pair.Value.Alias);
            }
            foreach (var pair in entity.CalculatedFields.Where(f => f.Value.FileOutput)) {
                Headers.Add(pair.Value.Alias);
            }
        }

        protected virtual void PrepareFooter(Entity entity) {
            FooterText = string.Empty;
        }

        protected virtual void PrepareType(Entity entity) {
            var builder = new DelimitedClassBuilder("Tfl" + entity.OutputName()) { IgnoreEmptyLines = true, Delimiter = _connection.Delimiter, IgnoreFirstLines = 0 };

            foreach (var pair in entity.Fields.Where(f => f.Value.FileOutput)) {
                builder.AddField(pair.Value.Alias, pair.Value.SystemType);
            }
            foreach (var pair in entity.CalculatedFields.Where(f => f.Value.FileOutput)) {
                builder.AddField(pair.Value.Alias, pair.Value.SystemType);
            }

            Type = builder.CreateRecordClass();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            PrepareHeader(_entity);
            PrepareType(_entity);
            PrepareFooter(_entity);

            var engine = new FluentFile(Type) { HeaderText = HeaderText, FooterText = FooterText };

            using (var file = engine.To(FileInfo.FullName)) {
                foreach (var row in rows) {
                    var record = row.ToObject(Type);
                    file.Write(record);
                }
            }
            yield break;
        }
    }
}