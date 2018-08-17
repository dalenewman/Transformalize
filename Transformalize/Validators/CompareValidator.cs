using System;
using System.Collections.Generic;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Validators {

    public class CompareValidator : BaseValidate {
        private readonly Func<IRow, bool> _validator;
        private readonly BetterFormat _betterFormat;
        private readonly string _type;
        public CompareValidator(string type, IContext context = null) : base(context) {
            _type = type;
            string explanation;
            switch (_type) {
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

            if (IsMissingContext()) {
                return;
            }

            var input = SingleInput();
            var help = Context.Field.Help;
            if (help == string.Empty) {
                help = $"{Context.Field.Label} {explanation} {Context.Operation.Value}.";
            }

            var isComparable = Constants.TypeDefaults()[input.Type] is IComparable;
            if (!isComparable) {
                Context.Error($"you can't use {type} validator on {input.Alias} field.  It's type is not comparable.");
                Run = false;
                return;
            }

            var isField = Context.Entity.FieldMatcher.IsMatch(Context.Operation.Value);
            if (isField) {
                var field = Context.Entity.GetField(Context.Operation.Value);
                _validator = delegate (IRow row) {
                    var inputValue = (IComparable)row[input];
                    var otherValue = row[field];
                    return type == "min" ? inputValue.CompareTo(otherValue) > -1 : inputValue.CompareTo(otherValue) < 1;
                };

            } else {
                var value = input.Convert(Context.Operation.Value);
                _validator = delegate (IRow row) {
                    var inputValue = (IComparable)row[input];
                    var otherValue = value;
                    return type == "min" ? inputValue.CompareTo(otherValue) > -1 : inputValue.CompareTo(otherValue) < 1;
                };
            }

            _betterFormat = new BetterFormat(context, help, Context.Entity.GetAllFields);
        }
        public override IRow Operate(IRow row) {
            if (IsInvalid(row, _validator(row))) {
                AppendMessage(row, _betterFormat.Format(row));
            }
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature(_type) { Parameters = new List<OperationParameter>(1) { new OperationParameter("value") } };
        }
    }
}