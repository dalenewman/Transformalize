using System.Collections.Generic;
using System.Text.RegularExpressions;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations.Transform
{
    public class RegexReplaceOperation : AbstractOperation {

        private readonly Regex _regex;
        private readonly string _inKey;
        private readonly string _outKey;
        private readonly string _replacement;
        private readonly int _count;

        public RegexReplaceOperation(string inKey, string outKey, string pattern, string replacement, int count) {
            _inKey = inKey;
            _outKey = outKey;
            _replacement = replacement;
            _count = count;
            _regex = new Regex(pattern, RegexOptions.Compiled);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (_count > 0) {
                    row[_outKey] = _regex.Replace(row[_inKey].ToString(), _replacement, _count);
                } else {
                    row[_outKey] = _regex.Replace(row[_inKey].ToString(), _replacement);
                }
                yield return row;
            }
        }
    }
}