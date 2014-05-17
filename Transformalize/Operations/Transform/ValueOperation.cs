using System.Collections.Generic;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class ValueOperation : ShouldRunOperation {
        private readonly IParameters _parameters;

        private readonly object _value;

        public ValueOperation(string outKey, string outType, string value, IParameters parameters)
            : base(string.Empty, outKey) {
            _parameters = parameters;
            if (!value.Equals(string.Empty)) {
                _value = Common.GetObjectConversionMap()[Common.ToSimpleType(outType)](value);
            } else if (!_parameters.Any()) {
                throw new TransformalizeException("The value transform method requires the value attribute to be set, or a parameter.");
            }

            Name = string.Format("ValueOperation ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = _value ?? row[_parameters[0].Name];
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }
    }
}