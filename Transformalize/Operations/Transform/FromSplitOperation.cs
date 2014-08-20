using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class FromSplitOperation : ShouldRunOperation {
        private readonly char[] _separator;

        private readonly KeyValuePair<string, IParameter>[] _parameters;

        public FromSplitOperation(string inKey, string separator, IParameters parameters)
            : base(inKey, string.Empty) {
            _separator = separator.ToCharArray();
            _parameters = parameters.ToEnumerable().ToArray();
            Name = string.Format("FromSplitOperation (in:{0})", inKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var values = row[InKey].ToString().Split(_separator);
                    if (values.Length > 0) {
                        for (var i = 0; i < values.Length; i++) {
                            var key = _parameters[i].Key;
                            row[key] = Common.ConversionMap[_parameters[i].Value.SimpleType](values[i]);
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