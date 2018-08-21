using System.Collections.Generic;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class CondenseTransform : StringTransform {

        private readonly char _char;
        private readonly IField _input;
        public CondenseTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext()) {
                return;
            }

            if (IsMissing(Context.Operation.Value)) {
                return;
            }

            if (Context.Operation.Value.Length > 1) {
                Warn("The condense transform can only accept 1 character as a parameter.");
                Run = false;
            }

            _input = SingleInput();

            _char = Context.Operation.Value[0];
        }

        public override IRow Operate(IRow row) {

            var value = GetString(row, _input);
            var found = false;
            var result = new List<char>(value.Length);
            var count = 0;

            foreach (var c in value) {
                if (c == _char) {
                    ++count;
                    switch (count) {
                        case 1:
                            result.Add(c);
                            break;
                        case 2:
                            found = true;
                            break;
                        default:
                            continue;
                    }
                } else {
                    count = 0;
                    result.Add(c);
                }
            }

            row[Context.Field] = found ? string.Concat(result) : value;
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("condense") { Parameters = new List<OperationParameter>(1) { new OperationParameter("value", " ") } };
        }
    }
}