using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Validators {

    public class MaxLengthValidator : StringValidate {
        private readonly Field _input;
        private readonly BetterFormat _betterFormat;

        public MaxLengthValidator(IContext context = null) : base(context) {
            if (IsMissingContext()) {
                return;
            }

            if (!Run)
                return;
            _input = SingleInput();
            var help = Context.Field.Help;
            if (help == string.Empty) {
                help = $"{Context.Field.Label} must have {Context.Operation.Length} or less characters.";
            }
            _betterFormat = new BetterFormat(context, help, Context.Entity.GetAllFields);
        }

        public override IRow Operate(IRow row) {
            if (IsInvalid(row, GetString(row, _input).Length > Context.Operation.Length)) {
                AppendMessage(row, _betterFormat.Format(row));
            }

            return row;
        }


        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("maxlength") { Parameters = new List<OperationParameter>(1) { new OperationParameter("length") } };
        }
    }
}