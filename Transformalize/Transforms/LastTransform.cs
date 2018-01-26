using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class LastTransform : BaseTransform {
        private readonly Field _input;

        public LastTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext() || context == null) {
                return;
            }

            if (LastMethodIsNot("split")) {
                return;
            }

            _input = SingleInput();
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = ((string[])row[_input]).Last();
            
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("last");
        }
    }
}