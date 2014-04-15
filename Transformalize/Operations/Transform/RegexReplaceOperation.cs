using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {
    public class RegexReplaceOperation : ShouldRunOperation {

        private readonly Regex _regex;
        private readonly string _replacement;
        private readonly int _count;

        public RegexReplaceOperation(string inKey, string outKey, string pattern, string replacement, int count)
            : base(inKey, outKey) {
            _replacement = replacement;
            _count = count;
            _regex = new Regex(pattern, RegexOptions.Compiled);
            Name = string.Format("RegexReplaceOperation ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    if (_count > 0) {
                        row[OutKey] = _regex.Replace(row[InKey].ToString(), _replacement, _count);
                    } else {
                        row[OutKey] = _regex.Replace(row[InKey].ToString(), _replacement);
                    }
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }
    }
}