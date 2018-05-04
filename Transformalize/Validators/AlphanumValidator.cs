using System.Text.RegularExpressions;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Validators {
    public class RegularExpressionValidator : StringValidate {
        private readonly Regex _regex;
        private readonly Field _input;
        private readonly BetterFormat _betterFormat;

        public RegularExpressionValidator(IContext context, string pattern, string explanation) : base(context) {
#if NETS10
            _regex = new Regex(pattern);
#else
            _regex = new Regex(pattern, RegexOptions.Compiled);
#endif

            if (!Run) {
                return;
            }
            _input = SingleInput();
            var help = context.Field.Help;
            if (help == string.Empty) {
                help = $"{context.Field.Label} {explanation}.";
            }
            _betterFormat = new BetterFormat(context, help, context.Entity.GetAllFields);
        }
        public override IRow Operate(IRow row) {
            if (IsInvalid(row, _regex.IsMatch(GetString(row, _input)))) {
                AppendMessage(row, _betterFormat.Format(row));
            }

            return row;
        }
    }
}