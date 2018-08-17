using System.Collections.Generic;
using System.Text.RegularExpressions;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Validators {
    public class RegularExpressionValidator : StringValidate {

        private readonly string _method;
        private readonly Regex _regex;
        private readonly Field _input;
        private readonly BetterFormat _betterFormat;

        public RegularExpressionValidator(string method, string pattern, string explanation, IContext context = null) : base(context) {


            _method = method;

            if (IsMissingContext()) {
                return;
            }
#if NETS10
            _regex = new Regex(pattern);
#else
            _regex = new Regex(pattern, RegexOptions.Compiled);
#endif

            if (!Run) {
                return;
            }
            _input = SingleInput();
            var help = Context.Field.Help;
            if (help == string.Empty) {
                help = $"{Context.Field.Label} {explanation}.";
            }
            _betterFormat = new BetterFormat(context, help, Context.Entity.GetAllFields);
        }
        public override IRow Operate(IRow row) {
            if (IsInvalid(row, _regex.IsMatch(GetString(row, _input)))) {
                AppendMessage(row, _betterFormat.Format(row));
            }

            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature(_method);
        }
    }
}