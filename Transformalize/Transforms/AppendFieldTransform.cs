using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class AppendFieldTransform : StringTransform {

        private readonly Field _input;
        private readonly IField _field;

        public AppendFieldTransform(IContext context, IField field) : base(context, "string") {

            _field = field;

            _input = SingleInput();

            if (Received() != "string") {
                Warn($"Appending to a {Received()} type in {Context.Field.Alias} converts it to a string.");
            }
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = row[_input] + GetString(row, _field);
            return row;
        }

    }
}