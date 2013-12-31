using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations.Transform
{
    public class TrimStartAppendOperation : AbstractOperation {
        private readonly string _inKey;
        private readonly string _outKey;
        private readonly string _separator;
        private readonly char[] _trimChars;

        public TrimStartAppendOperation(string inKey, string outKey, string trimChars, string separator) {
            _inKey = inKey;
            _outKey = outKey;
            _separator = separator;
            _trimChars = trimChars.ToCharArray();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows)
            {
                var orginal = row[_inKey].ToString();

                row[_outKey] = string.Join(_separator, new[] { orginal, orginal.TrimStart(_trimChars) });
                yield return row;
            }

        }
    }
}