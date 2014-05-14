using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Operations.Transform;

namespace Transformalize.Main {
    public class SplitIndexOperation : ShouldRunOperation {
        private readonly string _outType;
        private readonly int _count;
        private readonly int _index;
        private readonly char[] _sepArray;
        private readonly bool _convert;

        /// <summary>
        /// Splits the content into an array (limited by count).  Returns the element at the specified zero-based index.
        /// </summary>
        /// <param name="inKey"></param>
        /// <param name="outKey"></param>
        /// <param name="outType"></param>
        /// <param name="separator"></param>
        /// <param name="count"></param>
        /// <param name="index"></param>
        public SplitIndexOperation(string inKey, string outKey, string outType, string separator, int count, int index)
            : base(inKey, outKey) {
            _outType = outType;
            _count = count;
            _index = index;
            _sepArray = separator.ToCharArray();
            _convert = !outType.Equals("string");

            Name = "SplitIndex (" + outKey + ")";

        }


        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var content = row[InKey].ToString();
                    var split = _count > 0 ? content.Split(_sepArray, _count) : content.Split(_sepArray);
                    var length = split.Length;
                    if (!length.Equals(0)) {
                        if (_convert) {
                            row[OutKey] = Common.ConversionMap[_outType](_index == length ? split[_index - 1] : split[_index]);
                        } else {
                            row[OutKey] = _index == length ? split[_index - 1] : split[_index];
                        }
                    }
                }
                yield return row;
            }
        }
    }
}