using System.Collections.Generic;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;
using Transformalize.Transforms;

namespace Transformalize.Operations {
    public class ProcessTransform : AbstractOperation {
        private readonly ITransform[] _transforms;

        public ProcessTransform(Process process) {
            _transforms = process.Transforms;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                var r = row;
                foreach (var t in _transforms) {
                    t.Transform(ref r);
                }
                yield return r;
            }
        }
    }
}