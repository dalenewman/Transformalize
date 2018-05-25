using System.Linq;
using System.Text.RegularExpressions;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

    public class FromRegexTransform : StringTransform {

        private readonly Regex _regex;
        private readonly Field _input;
        private readonly Field[] _output;

        public FromRegexTransform(IContext context) : base(context, null) {

            ProducesFields = true;

            if (IsNotReceiving("string")) {
                return;
            }

            if (!context.Operation.Parameters.Any()) {
                Error($"The {context.Operation.Method} transform requires a collection of output fields.");
                Run = false;
                return;
            }

            if (context.Operation.Pattern == string.Empty) {
                Error("The fromregex method requires a regular expression pattern with groups defined.");
                Run = false;
                return;
            }

#if NETS10
            _regex = new Regex(context.Operation.Pattern);
#else
            _regex = new Regex(context.Operation.Pattern, RegexOptions.Compiled);
#endif

            _input = SingleInputForMultipleOutput();
            _output = MultipleOutput();
        }

        public override IRow Operate(IRow row) {

            var match = _regex.Match(GetString(row, _input));
            if (match.Success) {
                for (var i = 0; i < match.Groups.Count && i < _output.Length; i++) {
                    var group = match.Groups[i];
                    if (!group.Success)
                        continue;
                    row[_output[i]] = _output[i].Convert(@group.Captures[0].Value);
                }
            }

            return row;

        }

    }
}