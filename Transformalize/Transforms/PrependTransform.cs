using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

    public class PrependTransform : BaseTransform {

        private readonly Field _input;
        private readonly bool _isField;
        private readonly Field _field;

        public PrependTransform(IContext context) : base(context, "string") {
            _input = SingleInput();
            _isField = context.Entity.FieldMatcher.IsMatch(context.Operation.Value);
            if (_isField) {
                _field = context.Entity.GetField(context.Operation.Value);
            }
            Run = context.Operation.Value != Constants.DefaultSetting;

            if (Run) {
                if (Received() != "string") {
                    Warn($"Prepending to a {Received()} type in {context.Field.Alias} converts it to a string.");
                }
            } else {
                Warn($"Prepending nothing in {context.Field.Alias}.");
            }
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = (_isField ? row[_field].ToString() : Context.Operation.Value) + row[_input];
            Increment();
            return row;
        }

    }

}