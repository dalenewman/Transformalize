using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Rhino.Etl.Core.Files;
using Transformalize.Libs.FileHelpers.Enums;
using Transformalize.Libs.FileHelpers.RunTime;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Operations.Load {
    public class FileLoadOperation : AbstractOperation {

        private const string SPACE = " ";
        private readonly AbstractConnection _connection;
        private readonly Entity _entity;
        private readonly List<string> _strings = new List<string>();
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
                if (pair.Value.SimpleType.Equals("string"))
                    _strings.Add(pair.Value.Alias);
                Headers.Add(pair.Value.Alias.Replace(_connection.Delimiter, string.Empty));
            }
            foreach (var pair in entity.CalculatedFields.Where(f => f.Value.FileOutput)) {
                if (pair.Value.SimpleType.Equals("string"))
                    _strings.Add(pair.Value.Alias);
                Headers.Add(pair.Value.Alias.Replace(_connection.Delimiter, string.Empty));
            }
            HeaderText = string.Join(_connection.Delimiter, Headers);
        }

        protected virtual void PrepareFooter(Entity entity) {
            FooterText = string.Empty;
        }

        protected virtual void PrepareType(Entity entity) {
            var builder = new DelimitedClassBuilder("Tfl" + entity.OutputName()) { IgnoreEmptyLines = true, Delimiter = _connection.Delimiter, IgnoreFirstLines = 0 };

            foreach (var pair in entity.Fields.Where(f => f.Value.FileOutput)) {
                var field = new DelimitedFieldBuilder(pair.Value.Alias, pair.Value.SystemType);
                if (pair.Value.SimpleType.Equals("datetime")) {
                    field.Converter.Kind = ConverterKind.Date;
                    field.Converter.Arg1 = _connection.DateFormat;
                }
                builder.AddField(field);
            }
            foreach (var pair in entity.CalculatedFields.Where(f => f.Value.FileOutput)) {
                var field = new DelimitedFieldBuilder(pair.Value.Alias, pair.Value.SystemType);
                if (pair.Value.SimpleType.Equals("datetime")) {
                    field.Converter.Kind = ConverterKind.Date;
                    field.Converter.Arg1 = _connection.DateFormat;
                }
                builder.AddField(field);
            }

            Type = builder.CreateRecordClass();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            PrepareType(_entity);
            var engine = new FluentFile(Type);

            if (_connection.IncludeHeader) {
                PrepareHeader(_entity);
                engine.HeaderText = HeaderText;
            }

            if (_connection.IncludeFooter) {
                PrepareFooter(_entity);
                engine.FooterText = FooterText;
            }

            using (var file = engine.To(FileInfo.FullName)) {
                foreach (var row in rows) {
                    // you would think file helpers would handle this...
                    foreach (var s in _strings) {
                        row[s] = row[s].ToString().Replace(_connection.Delimiter, SPACE);
                    }
                    var record = row.ToObject(Type);
                    file.Write(record);
                }
            }
            yield break;
        }
    }
}