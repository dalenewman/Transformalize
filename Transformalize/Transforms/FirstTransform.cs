using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class FirstTransform : BaseTransform {
        private readonly Field _input;

        public FirstTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext()) {
                return;
            }

            if (LastMethodIsNot("split","sort","reverse")) {
                return;
            }

            _input = SingleInput();
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = ((string[])row[_input]).FirstOrDefault() ?? string.Empty;
            
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("first");
        }
    }
}