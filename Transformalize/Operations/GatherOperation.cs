using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations {
    /// <summary>
    /// Because the input and the output are the same, gather calculated columns into array before writing.
    /// Also remove no longer needed parameters in order to release memory
    /// </summary>
    public class GatherOperation : AbstractOperation {

        private readonly string[] _keys;

        public GatherOperation(Process process) {
            _keys = process.Parameters.ToEnumerable().Where(p => !p.Value.HasValue()).Select(p => p.Key).ToArray();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            var gathered = new List<Row>();
            foreach (var row in rows) {
                foreach (var key in _keys) {
                    row.Remove(key);
                }
                gathered.Add(row);
            }
            return gathered;
        }
    }
}