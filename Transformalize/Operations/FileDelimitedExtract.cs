using System.Collections.Generic;
using System.Linq;
using Rhino.Etl.Core.Files;
using Transformalize.Libs.FileHelpers.RunTime;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations {
    public class FileDelimitedExtract : AbstractOperation {
        private readonly Entity _entity;
        private readonly Field[] _fields;
        private readonly ConversionFactory _conversionFactory = new ConversionFactory();

        public FileDelimitedExtract(Entity entity) {
            _entity = entity;
            _fields = new FieldSqlWriter(_entity.Fields).Input().Context().ToEnumerable().OrderBy(f => f.Index).ToArray();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            var cb = new DelimitedClassBuilder("Tfl" + _entity.OutputName()) { IgnoreEmptyLines = true, Delimiter = _entity.InputConnection.Delimiter };
            
            foreach (var field in _fields) {
                cb.AddField(field.Alias, typeof(string));
            }

            using (var file = new FluentFile(cb.CreateRecordClass()).From(_entity.InputConnection.File)) {
                foreach (var row in from object obj in file select Row.FromObject(obj)) {
                    foreach (var field in _fields) {
                        if(field.SimpleType != "string")
                            row[field.Alias] = _conversionFactory.Convert(row[field.Alias].ToString(), field.SimpleType, field.Default);
                    }
                    yield return row;
                }
            }
        }


    }
}