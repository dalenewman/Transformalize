using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Validators
{
    public class LengthValidator : StringValidate {
        private readonly Field _input;
        private readonly BetterFormat _betterFormat;

        public LengthValidator(IContext context) : base(context) {
            if (!Run)
                return;
            _input = SingleInput();
            var help = context.Field.Help;
            if (help == string.Empty) {
                help = $"{context.Field.Label} must be {context.Operation.Length} characters.";
            }
            _betterFormat = new BetterFormat(context, help, context.Entity.GetAllFields);
        }

        public override IRow Operate(IRow row) {
            var valid = GetString(row, _input).Length == Context.Operation.Length;
            row[ValidField] = valid;
            if (!valid) {
                AppendMessage(row, _betterFormat.Format(row));
            }
            
            return row;
        }
    }
}