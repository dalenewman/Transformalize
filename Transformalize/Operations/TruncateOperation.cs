using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Operations.Transform;

namespace Transformalize.Operations {
    public class TruncateOperation : ShouldRunOperation {
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly Dictionary<string, int> _lengthMap = new Dictionary<string, int>();
        private readonly List<string> _aliases = new List<string>();
        private int _count;
        private readonly Dictionary<string, byte> _truncatedFields = new Dictionary<string, byte>();
        private const Byte HIT = default(byte);

        public TruncateOperation(Fields fields, IEnumerable<KeyValuePair<string, Field>> calculatedFields = null)
            : base(string.Empty, string.Empty) {

            _aliases = fields.Where(kv => kv.Value.SimpleType.Equals("string")).Select(kv => kv.Key).ToList();
            foreach (var alias in _aliases) {
                var value = fields[alias].Length.Equals("max", IC) ? Int32.MaxValue.ToString(CultureInfo.InvariantCulture) : fields[alias].Length;
                if (CanChangeType(value, typeof(int))) {
                    _lengthMap[alias] = Convert.ToInt32(value);
                } else {
                    throw new TransformalizeException("Can not change field {0}'s length of '{0}' to an integer.  Please use an integer or the keyword: max.", alias, value);
                }
            }

            if (calculatedFields != null) {
                foreach (var kv in calculatedFields.Where(cf => cf.Value.SimpleType.Equals("string"))) {
                    if (!_aliases.Contains(kv.Key)) {
                        _aliases.Add(kv.Key);
                        var value = kv.Value.Length.Equals("max", IC) ? Int32.MaxValue.ToString(CultureInfo.InvariantCulture) : kv.Value.Length;
                        if (CanChangeType(value, typeof(int))) {
                            _lengthMap[kv.Key] = Convert.ToInt32(value);
                        } else {
                            throw new TransformalizeException("Can not change field {0}'s length of '{1}' to an integer.  Please use an integer or the keyword: max.", kv.Value.Alias, value);
                        }
                    }
                }
            }

            base.OnFinishedProcessing += StringLengthOperation_OnFinishedProcessing;
        }

        void StringLengthOperation_OnFinishedProcessing(IOperation obj) {
            if (_count > 0)
                Warn("Please address truncated {0} fields: {1}.", _count, string.Join(", ", _truncatedFields.Select(kv => kv.Key)));
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                foreach (var field in _aliases) {
                    var value = (row[field] ?? string.Empty).ToString();
                    if (value.Length > _lengthMap[field]) {
                        row[field] = value.Substring(0, _lengthMap[field]);
                        Interlocked.Increment(ref _count);
                        _truncatedFields[field] = HIT;
                    }
                }
                yield return row;
            }
        }
    }
}