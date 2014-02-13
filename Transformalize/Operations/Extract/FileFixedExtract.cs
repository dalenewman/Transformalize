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

namespace Transformalize.Operations.Extract {
    public class FileFixedExtract : AbstractOperation {
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly Entity _entity;
        private readonly int _top;
        private readonly Field[] _fields;
        private readonly string _fullName;
        private readonly string _name;
        private int _counter;

        public FileFixedExtract(Entity entity, int top) : this(entity, entity.InputConnection.File, top) { }

        public FileFixedExtract(Entity entity, string file, int top) {
            _entity = entity;
            _top = top;
            _fields = new FieldSqlWriter(_entity.Fields).Input().Context().ToEnumerable().OrderBy(f => f.Index).ToArray();

            var fileInfo = new FileInfo(file);
            _fullName = fileInfo.FullName;
            _name = fileInfo.Name;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            var cb = new FixedLengthClassBuilder("Tfl" + _entity.Alias) { IgnoreEmptyLines = true, FixedMode = FixedMode.AllowVariableLength };
            foreach (var field in _fields) {
                var length = field.Length.Equals("max", IC) ? Int32.MaxValue : Convert.ToInt32(field.Length);
                cb.AddField(field.Alias, length, typeof(string));
            }

            Info("Reading {0}", _name);

            if (_top > 0) {
                using (var file = new FluentFile(cb.CreateRecordClass()).From(_fullName).OnError(_entity.InputConnection.ErrorMode)) {
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
                using (var file = new FluentFile(cb.CreateRecordClass()).From(_fullName).OnError(_entity.InputConnection.ErrorMode)) {
                    foreach (var obj in file) {
                        yield return Row.FromObject(obj);
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