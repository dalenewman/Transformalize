using System.Collections.Generic;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;
using Transformalize.Main.Parameters;

namespace Transformalize.Operations.Transform {
    public class InsertOperation : ShouldRunOperation {
        private readonly int _startIndex;
        private readonly string _value;
        private readonly IParameter _parameter;
        private readonly bool _useParameter;

        public InsertOperation(string inKey, string outKey, int startIndex, string value, IParameter parameter)
            : base(inKey, outKey) {
            _startIndex = startIndex;
            _value = value;
            _parameter = parameter;
            if (value.Equals(string.Empty) && parameter != null) {
                if (parameter.HasValue()) {
                    _value = parameter.Value.ToString();
                } else {
                    _useParameter = true;
                }
            }
            Name = string.Format("Insert ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var rowValue = row[InKey].ToString();
                    if (rowValue.Length > _startIndex) {
                        if (_useParameter) {
                            row[OutKey] = rowValue.Insert(_startIndex, row[_parameter.Name].ToString());
                        } else {
                            row[OutKey] = rowValue.Insert(_startIndex, _value);
                        }
                    }
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }
    }
}