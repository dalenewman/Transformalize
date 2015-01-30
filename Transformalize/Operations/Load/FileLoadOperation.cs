using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Rhino.Etl.Core.Files;
using Transformalize.Libs.FileHelpers.Converters;
using Transformalize.Libs.FileHelpers.Enums;
using Transformalize.Libs.FileHelpers.RunTime;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Logging;
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Operations.Load {

    public class GuidConverter : ConverterBase {
        public override object StringToField(string @from) {
            return Guid.Parse(@from);
        }
    }

    public class ByteArrayConverter : ConverterBase {
        public override object StringToField(string @from) {
            return Common.HexStringToByteArray(@from);
        }

        public override string FieldToString(object @from) {
            return Common.BytesToHexString((byte[])@from);
        }
    }

    public class FileLoadOperation : AbstractOperation {

        private const char SPACE = ' ';
        private readonly AbstractConnection _connection;
        private readonly Entity _entity;
        private readonly List<string> _strings = new List<string>();
        private readonly bool _isCsv;
        private readonly Fields _fileFields = new Fields();
        private readonly string[] _stringFields;
        private readonly Field[] _mapFields = new Field[0];

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
            _isCsv = _connection.File.ToLower().EndsWith(".csv");

            _fileFields.Add(_entity.Fields.WithFileOutput());
            _fileFields.Add(_entity.CalculatedFields.WithFileOutput());
            _stringFields = _fileFields.WithString().Aliases().ToArray();
            _mapFields = _fileFields.WithIdentifiers().ToArray();

            if (FileInfo.DirectoryName != null && !Directory.Exists(FileInfo.DirectoryName)) {
                Info("Creating Output Folder(s).");
                Directory.CreateDirectory(FileInfo.DirectoryName);
            }

            if (FileInfo.Exists)
                return;
            TflLogger.Warn(entity.ProcessName, entity.Alias, "Output file already exists.  Deleting...");

            FileInfo.Delete();
        }

        protected virtual void PrepareHeader(Entity entity) {
            if (_connection.Header.Equals(Common.DefaultValue)) {
                var delimiter = _connection.Delimiter.ToString(CultureInfo.InvariantCulture);
                foreach (var field in _fileFields) {
                    if (field.SimpleType.Equals("string"))
                        _strings.Add(field.Alias);
                    Headers.Add(field.Alias.Replace(delimiter, string.Empty));
                }
                HeaderText = string.Join(delimiter, Headers);
            } else {
                HeaderText = _connection.Header;
            }
        }

        protected virtual void PrepareFooter(Entity entity) {
            FooterText = _connection.Footer;
        }

        protected virtual void PrepareType(Entity entity) {
            var builder = new DelimitedClassBuilder("Tfl" + entity.OutputName()) { IgnoreEmptyLines = true, Delimiter = _connection.Delimiter.ToString(CultureInfo.InvariantCulture), IgnoreFirstLines = 0 };

            foreach (var f in _fileFields) {
                var field = new DelimitedFieldBuilder(f.Identifier, f.SystemType);
                if (f.SimpleType.Equals("datetime")) {
                    field.Converter.Kind = ConverterKind.Date;
                    field.Converter.Arg1 = _connection.DateFormat;
                }
                if (f.SimpleType.Equals("guid")) {
                    field.Converter.TypeName = "Transformalize.Operations.Load.GuidConverter";
                }
                if (f.SimpleType.Equals("byte[]") || f.SimpleType.Equals("rowversion")) {
                    field.Converter.TypeName = "Transformalize.Operations.Load.ByteArrayConverter";
                }
                if (_isCsv) {
                    field.FieldQuoted = true;
                    field.QuoteChar = '"';
                    field.QuoteMode = QuoteMode.OptionalForBoth;
                    field.QuoteMultiline = MultilineMode.NotAllow;
                }
                builder.AddField(field);
            }
            Type = builder.CreateRecordClass();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            PrepareType(_entity);
            FluentFile engine;

            try {
                engine = new FluentFile(Type) {
                    Encoding = _connection.Encoding.Equals("utf-8w/obom") ?
                        new UTF8Encoding(false) :
                        Encoding.GetEncoding(_connection.Encoding)
                };
            } catch (Exception ex) {
                throw new TransformalizeException(ProcessName, EntityName, ex.Message);
            }

            if (!_connection.Header.Equals(string.Empty)) {
                PrepareHeader(_entity);
                engine.HeaderText = HeaderText;
            }

            if (!_connection.Footer.Equals(string.Empty)) {
                PrepareFooter(_entity);
                engine.FooterText = FooterText;
            }

            using (var file = engine.To(FileInfo.FullName)) {
                foreach (var row in rows) {
                    foreach (var field in _stringFields) {
                        var value = row[field].ToString();
                        if (_isCsv) {
                            row[field] = value.Replace("\r\n", "\n");
                        } else if (_connection.Delimiter != SPACE) {
                            row[field] = value.Replace(_connection.Delimiter, SPACE);
                        }
                    }
                    foreach (var field in _mapFields) {
                        row[field.Identifier] = row[field.Alias];
                    }
                    var record = row.ToObject(Type);
                    file.Write(record);
                }
            }
            yield break;
        }
    }
}