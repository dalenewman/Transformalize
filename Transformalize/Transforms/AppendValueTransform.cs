using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class AppendValueTransform : StringTransform {

        private readonly Field _input;

        public AppendValueTransform(IContext context) : base(context, "string") {

            _input = SingleInput();

            if (Received() != "string") {
                Warn($"Appending to a {Received()} type in {Context.Field.Alias} converts it to a string.");
            }
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = row[_input] + Context.Operation.Value;
            return row;
        }



    }
}