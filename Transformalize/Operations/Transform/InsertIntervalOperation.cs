using System.Collections.Generic;
using System.Text.RegularExpressions;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class InsertIntervalOperation : ShouldRunOperation {

        private readonly Regex _regex;
        private readonly string _replacement;
        private readonly char[] _valueArray;

        public InsertIntervalOperation(string inKey, string outKey, int interval, string value)
            : base(inKey, outKey) {

            if (interval <= 0) {
                throw new TransformalizeException("Insert Interval operation for {0} must have interval > 0.", outKey);
            }

            _valueArray = value.ToCharArray();
            _regex = new Regex(".{" + interval + "}", RegexOptions.Compiled);
            _replacement = "$0" + value;

            Name = "InsertInterval (" + outKey + ")";
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = _regex.Replace(row[InKey].ToString(), _replacement).TrimEnd(_valueArray);
                }
                yield return row;
            }
        }
    }
}