using System;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Validators {

    public class CompareValidator : BaseValidate {
        private readonly Func<IRow, bool> _validator;
        private readonly BetterFormat _betterFormat;

        public CompareValidator(IContext context, string type) : base(context) {

            string explanation;
            switch (type) {
                case "min":
                    explanation = "must be greater than or equal to";
                    break;
                case "max":
                    explanation = "must be less than or equal to";
                    break;
                default:
                    Run = false;
                    return;
            }

            var input = SingleInput();
            var help = context.Field.Help;
            if (help == string.Empty) {
                help = $"{context.Field.Label} {explanation} {context.Operation.Value}.";
            }

            var isComparable = Constants.TypeDefaults()[input.Type] is IComparable;
            if (!isComparable) {
                context.Error($"you can't use {type} validator on {input.Alias} field.  It's type is not comparable.");
                Run = false;
                return;
            }

            var isField = context.Entity.FieldMatcher.IsMatch(context.Operation.Value);
            if (isField) {
                var field = context.Entity.GetField(context.Operation.Value);
                _validator = delegate (IRow row) {
                    var inputValue = (IComparable)row[input];
                    var otherValue = row[field];
                    return type == "min" ? inputValue.CompareTo(otherValue) > -1 : inputValue.CompareTo(otherValue) < 1;
                };

            } else {
                var value = input.Convert(context.Operation.Value);
                _validator = delegate (IRow row) {
                    var inputValue = (IComparable)row[input];
                    var otherValue = value;
                    return type == "min" ? inputValue.CompareTo(otherValue) > -1 : inputValue.CompareTo(otherValue) < 1;
                };
            }

            _betterFormat = new BetterFormat(context, help, context.Entity.GetAllFields);
        }
        public override IRow Operate(IRow row) {
            if (IsInvalid(row, _validator(row))) {
                AppendMessage(row, _betterFormat.Format(row));
            }
            return row;
        }
    }
}