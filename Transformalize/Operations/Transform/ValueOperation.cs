using System.Collections.Generic;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class ValueOperation : ShouldRunOperation {

        private readonly object _value;

        public ValueOperation(string outKey, string outType, string value)
            : base(string.Empty, outKey) {
            _value = Common.GetObjectConversionMap()[Common.ToSimpleType(outType)](value);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = _value;
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }
    }
}