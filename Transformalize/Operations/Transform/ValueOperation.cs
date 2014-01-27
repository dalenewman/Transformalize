using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class ValueOperation : TflOperation {

        private readonly object _value;

        public ValueOperation(string outKey, string outType, string value)
            : base(string.Empty, outKey) {
            _value = Common.GetObjectConversionMap()[Common.ToSimpleType(outType)](value);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = _value;
                }
                yield return row;
            }
        }
    }
}