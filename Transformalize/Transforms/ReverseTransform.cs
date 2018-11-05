using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class ReverseTransform : BaseTransform {
        private readonly Field _input;
        private readonly Func<object, string[]> _sort;
        public ReverseTransform(IContext context = null) : base(context, "object") {
            if (IsMissingContext()) {
                return;
            }

            if (LastMethodIsNot("split")) {
                return;
            }

            _input = SingleInput();
            _sort = o => ((string[])o).Reverse().ToArray();
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _sort(row[_input]);
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("reverse");
        }
    }
}