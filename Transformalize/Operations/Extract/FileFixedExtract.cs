using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Rhino.Etl.Core.Files;
using Transformalize.Libs.FileHelpers.Attributes;
using Transformalize.Libs.FileHelpers.Enums;
using Transformalize.Libs.FileHelpers.RunTime;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Operations.Extract {

    public class FileFixedExtract : AbstractOperation {

        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;

        private readonly Entity _entity;
        private readonly int _top;
        private readonly Field[] _fields;
        private readonly string _fullName;
        private readonly string _name;
        private readonly ErrorMode _errorMode;
        private readonly int _ignoreFirstLines;
        private readonly FixedLengthClassBuilder _classBuilder;

        private int _counter;

        public FileFixedExtract(Entity entity, AbstractConnection connection, int top) {

            var fileInfo = new FileInfo(connection.File);

            _entity = entity;
            _top = top;
            _fields = new FieldSqlWriter(_entity.Fields).Input().Context().OrderedFields().ToArray();
            _fullName = fileInfo.FullName;
            _name = fileInfo.Name;
            _errorMode = connection.ErrorMode;
            _ignoreFirstLines = connection.Start - 1;

            _classBuilder = new FixedLengthClassBuilder("Tfl" + _entity.Alias) {
                IgnoreEmptyLines = true,
                IgnoreFirstLines = _ignoreFirstLines
            };
            foreach (var field in _fields) {
                var length = field.Length.Equals("max", IC) ? int.MaxValue : Convert.ToInt32(field.Length.Equals(string.Empty) ? "64" : field.Length);
                var builder = new FixedFieldBuilder(field.Alias, length, typeof(string)) {
                    FieldNullValue = new String(' ', length)
                };
                _classBuilder.AddField(builder);
            }

        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            Info("Reading {0}", _name);

            if (_top > 0) {
                using (var file = new FluentFile(_classBuilder.CreateRecordClass()).From(_fullName).OnError(_errorMode)) {
                    foreach (var obj in file) {
                        var row = Row.FromObject(obj);
                        row["TflFileName"] = _fullName;
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
                using (var file = new FluentFile(_classBuilder.CreateRecordClass()).From(_fullName).OnError(_errorMode)) {
                    foreach (var obj in file) {
                        var row = Row.FromObject(obj);
                        row["TflFileName"] = _fullName;
                        yield return row;
                    }
                    HandleErrors(file);
                }
            }

        }

        private bool HandleErrors(FileEngine file) {
            if (!file.HasErrors)
                return true;

            var errorInfo = new FileInfo(Common.GetTemporaryFolder(_entity.ProcessName).TrimEnd(new[] { '\\' }) + @"\" + _name + ".errors.txt");
            file.OutputErrors(errorInfo.FullName);
            Warn("Errors sent to {0}.", errorInfo.Name);
            return false;
        }

    }
}