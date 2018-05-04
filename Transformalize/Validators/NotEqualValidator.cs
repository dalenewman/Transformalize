using System;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Validators {
    public class NotEqualValidator : BaseValidate {

        private readonly object _value;
        private readonly Func<IRow, bool> _validator;
        private readonly BetterFormat _betterFormat;

        public NotEqualValidator(IContext context) : base(context) {
            if (!Run)
                return;

            Field[] rest;
            bool sameTypes;
            var input = MultipleInput();
            var first = input.First();

            if (context.Operation.Value == Constants.DefaultSetting) {
                rest = input.Skip(1).ToArray();
                sameTypes = rest.All(f => f.Type == first.Type);
            } else {
                _value = first.Convert(context.Operation.Value);
                rest = input.ToArray();
                sameTypes = input.All(f => f.Type == first.Type);
            }

            if (sameTypes) {
                if (_value == null) {
                    _validator = row => !rest.All(f => row[f].Equals(row[first]));
                } else {
                    _validator = row => !rest.All(f => row[f].Equals(_value));
                }
            } else {
                _validator = row => true;
            }

            var help = context.Field.Help;
            if (help == string.Empty) {
                if (_value == null) {
                    help = $"{context.Field.Label} must not equal {{{first.Alias}}} in {first.Label}.";
                } else {
                    help = $"{context.Field.Label} must not equal {_value}.";
                }
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