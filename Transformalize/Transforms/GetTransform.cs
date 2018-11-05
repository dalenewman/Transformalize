using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class GetTransform : BaseTransform {

        private readonly Field _input;
        private readonly int _index;

        public GetTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext()) {
                return;
            }

            if (LastMethodIsNot("split", "sort", "reverse")) {
                return;
            }

            if (IsMissing(Context.Operation.Value)) {
                return;
            }

            if (int.TryParse(Context.Operation.Value, out var index)) {
                _index = index;
            } else {
                Context.Error($"The parameter {Context.Operation.Value} in invalid. The get() transform only accepts an integer.");
                Run = false;
            }

            _input = SingleInput();
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = ((string[])row[_input]).ElementAtOrDefault(_index) ?? string.Empty;
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("get") { Parameters = new List<OperationParameter>(1) { new OperationParameter("value") } };
        }
    }
}