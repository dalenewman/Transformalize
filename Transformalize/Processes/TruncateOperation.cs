using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Processes
{
    public class TruncateOperation : AbstractOperation {
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly Dictionary<string, int> _lengthMap = new Dictionary<string, int>();
        private readonly List<string> _aliases = new List<string>();
        private int _count;

        public TruncateOperation(Fields fields, Fields calculatedFields = null) {

            _aliases = fields.Where(kv => kv.Value.SimpleType.Equals("string")).Select(kv => kv.Key).ToList();
            foreach (var alias in _aliases) {
                _lengthMap[alias] = fields[alias].Length.Equals("max", IC) ? Int32.MaxValue : Convert.ToInt32(fields[alias].Length);
            }

            if (calculatedFields != null) {
                foreach (var kv in calculatedFields) {
                    if (!_aliases.Contains(kv.Key)) {
                        _aliases.Add(kv.Key);
                        _lengthMap[kv.Key] = kv.Value.Length.Equals("max", IC) ? Int32.MaxValue : Convert.ToInt32(kv.Value.Length);
                    }
                }
            }

            base.OnFinishedProcessing += StringLengthOperation_OnFinishedProcessing;

        }

        void StringLengthOperation_OnFinishedProcessing(IOperation obj) {
            if(_count > 0)
                Warn("Truncated {0} fields.", _count);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                foreach (var field in _aliases) {
                    var value = (row[field] ?? string.Empty).ToString();
                    if (value.Length > _lengthMap[field]) {
                        row[field] = value.Substring(0, _lengthMap[field]);
                        Interlocked.Increment(ref _count);
                    }
                }
                yield return row;
            }
        }
    }
}