using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Rhino.Etl.Core.Files;
using Transformalize.Libs.FileHelpers.Enums;
using Transformalize.Libs.FileHelpers.RunTime;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations.Extract {

    public class FileDelimitedExtract : AbstractOperation {
        private readonly Entity _entity;
        private readonly int _top;
        private readonly Field[] _fields;
        private readonly string _fullName;
        private readonly string _name;
        private int _counter;

        public FileDelimitedExtract(Entity entity, int top) : this(entity, entity.InputConnection.File, top) { }

        public FileDelimitedExtract(Entity entity, string file, int top) {
            _entity = entity;
            _top = top;
            _fields = new FieldSqlWriter(_entity.Fields).Input().Context().ToEnumerable().OrderBy(f => f.Index).ToArray();

            var fileInfo = new FileInfo(file);
            _fullName = fileInfo.FullName;
            _name = fileInfo.Name;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            var ignoreFirstLines = _entity.InputConnection.Start - 1;
            var cb = new DelimitedClassBuilder("Tfl" + _entity.OutputName()) {
                IgnoreEmptyLines = true,
                Delimiter = _entity.InputConnection.Delimiter,
                IgnoreFirstLines = ignoreFirstLines
            };

            foreach (var field in _fields) {
                if (!field.QuotedWith.Equals(string.Empty)) {
                    cb.AddField(new DelimitedFieldBuilder(field.Alias, typeof(string)) {
                        FieldQuoted = true,
                        QuoteChar = field.QuotedWith[0],
                        QuoteMode = QuoteMode.OptionalForRead,
                        FieldOptional = field.Optional
                    });
                } else {
                    cb.AddField(new DelimitedFieldBuilder(field.Alias, typeof(string)) {
                        FieldOptional = field.Optional
                    });
                }
            }

            Info("Reading {0}", _name);

            var conversionMap = Common.GetObjectConversionMap();

            if (_top > 0) {
                using (var file = new FluentFile(cb.CreateRecordClass()).From(_fullName).OnError(_entity.InputConnection.ErrorMode)) {
                    foreach (var row in from object obj in file select Row.FromObject(obj)) {
                        row["TflFileName"] = _fullName;
                        foreach (var field in _fields.Where(f => !f.SimpleType.Equals("string"))) {
                            var value = row[field.Alias] == null || !field.SimpleType.Equals("string") && row[field.Alias].ToString().Equals(string.Empty) ? field.Default : row[field.Alias];
                            row[field.Alias] = conversionMap[field.SimpleType](value);
                        }
                        if (_counter < _top) {
                            Interlocked.Increment(ref _counter);
                            yield return row;
                        } else {
                            yield break;
                        }
                    }
                    HandleErrors(file);
                }

            } else {
                using (var file = new FluentFile(cb.CreateRecordClass()).From(_fullName).OnError(_entity.InputConnection.ErrorMode)) {
                    foreach (var row in from object obj in file select Row.FromObject(obj)) {
                        row["TflFileName"] = _fullName;
                        foreach (var field in _fields.Where(f => !f.SimpleType.Equals("string"))) {
                            var value = row[field.Alias] == null || !field.SimpleType.Equals("string") && row[field.Alias].ToString().Equals(string.Empty) ? field.Default : row[field.Alias];
                            row[field.Alias] = conversionMap[field.SimpleType](value);
                        }
                        yield return row;
                    }
                    HandleErrors(file);
                }
            }

        }

        private void HandleErrors(FileEngine file) {
            if (!file.HasErrors)
                return;

            var errorInfo = new FileInfo(Common.GetTemporaryFolder(_entity.ProcessName).TrimEnd(new[] { '\\' }) + @"\" + _name + ".errors.txt");
            file.OutputErrors(errorInfo.FullName);
            Warn("Errors sent to {0}.", errorInfo.Name);
            return;
        }
    }
}