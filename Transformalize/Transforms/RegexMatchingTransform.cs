using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class RegexMatchingTransform : StringTransform {

        private readonly Field _input;
        private readonly Regex _regex;
        private readonly string _default;

        public RegexMatchingTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext()) {
                return;
            }

            if (IsNotReceiving("string")) {
                return;
            }

            if (IsMissing(Context.Operation.Pattern)) {
                return;
            }

            _default = Context.Field.Default == Constants.DefaultSetting ? Constants.StringDefaults()[Context.Field.Type] : Context.Field.Convert(Context.Field.Default).ToString();

            _input = SingleInput();

#if NETS10
            _regex = new Regex(Context.Operation.Pattern);
#else
            _regex = new Regex(Context.Operation.Pattern, RegexOptions.Compiled);
#endif

        }

        public override IRow Operate(IRow row) {
            var matches = _regex.Matches(GetString(row, _input));
            row[Context.Field] = matches.Count > 0 ? string.Concat(matches.Cast<Match>().Select(m => m.Value)) : _default;
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            return new[] {
                new OperationSignature("matching") {
                    Parameters =  new List<OperationParameter> {
                        new OperationParameter("pattern")
                    }
                }
            };
        }
    }
}