using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Libs.Sqloogle.Operations.Support {

    public class KeyCheckOperation : AbstractOperation {

        private readonly List<string> _keys = new List<string>();

        public KeyCheckOperation(IEnumerable<string> keys)
        {
            _keys.AddRange(keys);    
        }

        public KeyCheckOperation AddRange(IEnumerable<string> keys)
        {
            _keys.AddRange(keys);
            return this;
        }

        public KeyCheckOperation AddParams(params string[] keys)
        {
            _keys.AddRange(keys);
            return this;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            foreach (var row in rows) {
                foreach (var key in _keys) {
                    Guard.Against(!row.Contains(key), string.Format("Row must contain {0} key", key));
                }
                yield return row;
            }
        }
    }
}
