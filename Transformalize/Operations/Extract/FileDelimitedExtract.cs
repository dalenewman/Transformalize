using System;
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
using Transformalize.Main.Providers;

namespace Transformalize.Operations.Extract {

    public class FileDelimitedExtract : AbstractOperation {

        private readonly Entity _entity;
        private readonly int _top;
        private readonly Field[] _fields;
        private readonly string _fullName;
        private readonly string _name;
        private readonly int _ignoreFirstLines;
        private readonly string _delimiter;
        private readonly ErrorMode _errorMode;

        private int _counter;

        public FileDelimitedExtract(Entity entity, AbstractConnection connection, int top) {

            var fileInfo = new FileInfo(connection.File);

            _entity = entity;
            _top = top;
            _fields = entity.Fields.WithInput().ToArray();
            _delimiter = connection.Delimiter;
            _fullName = fileInfo.FullName;
            _name = fileInfo.Name;
            _ignoreFirstLines = connection.Start - 1;
            _errorMode = connection.ErrorMode;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            var cb = new DelimitedClassBuilder("Tfl" + _entity.OutputName()) {
                IgnoreEmptyLines = true,
                Delimiter = _delimiter,
                IgnoreFirstLines = _ignoreFirstLines
            };

            foreach (var field in _fields) {
                if (field.IsQuoted()) {
                    cb.AddField(new DelimitedFieldBuilder(field.Identifier, typeof(string)) {
                        FieldQuoted = true,
                        QuoteChar = field.QuotedWith,
                        QuoteMode = QuoteMode.OptionalForRead,
                        FieldOptional = field.Optional
                    });
                } else {
                    cb.AddField(new DelimitedFieldBuilder(field.Identifier, typeof(string)) {
                        FieldOptional = field.Optional
                    });
                }
            }

            Info("Reading {0}", _name);

            var conversionMap = Common.GetObjectConversionMap();

            if (_top > 0) {
                using (var file = new FluentFile(cb.CreateRecordClass()).From(_fullName).OnError(_errorMode)) {
                    foreach (var row in from object obj in file select Row.FromObject(obj)) {
                        ProcessRow(row, _fields, _fullName, conversionMap);
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
                using (var file = new FluentFile(cb.CreateRecordClass()).From(_fullName).OnError(_errorMode)) {
                    foreach (var row in from object obj in file select Row.FromObject(obj)) {
                        ProcessRow(row, _fields, _fullName, conversionMap);
                        yield return row;
                    }
                    HandleErrors(file);
                }
            }

        }

        private static void ProcessRow(Row row, IEnumerable<Field> fields, string fileName, IReadOnlyDictionary<string, Func<object, object>> conversionMap) {
            foreach (var field in fields) {
                if (field.SimpleType.Equals("string") && !field.Alias.Equals(field.Identifier)) {
                    row[field.Alias] = row[field.Identifier];
                    row.Remove(field.Identifier);
                } else {
                    var value = row[field.Identifier] == null || !field.SimpleType.Equals("string") && row[field.Identifier].ToString().Equals(string.Empty) ? field.Default : row[field.Identifier];
                    row[field.Alias] = conversionMap[field.SimpleType](value);
                    if (!field.Alias.Equals(field.Identifier)) {
                        row.Remove(field.Identifier);
                    }
                }
            }
            row["TflFileName"] = fileName;
        }

        private void HandleErrors(FileEngine file) {
            if (!file.HasErrors)
                return;

            var errorInfo = new FileInfo(Common.GetTemporaryFolder(_entity.ProcessName).TrimEnd(new[] { '\\' }) + @"\" + _name + ".errors.txt");
            file.OutputErrors(errorInfo.FullName);
            Warn("Errors sent to {0}.", errorInfo.Name);
        }
    }
}