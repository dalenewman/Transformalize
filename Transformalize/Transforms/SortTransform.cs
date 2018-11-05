using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class SortTransform : BaseTransform {
        private readonly Field _input;
        private readonly Func<object, string[]> _sort;
        public SortTransform(IContext context = null) : base(context, "object") {
            if (IsMissingContext()) {
                return;
            }

            if (LastMethodIsNot("split")) {
                return;
            }

            _input = SingleInput();
            if (Context.Operation.Direction == ("asc")) {
                _sort = o => ((string[])o).OrderBy(s => s).ToArray();
            } else {
                _sort = o => ((string[])o).OrderByDescending(s => s).ToArray();
            }
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _sort(row[_input]);
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("sort") { Parameters = new List<OperationParameter>(1) { new OperationParameter("direction", "asc") } };
        }
    }
}