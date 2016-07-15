using System.Text.RegularExpressions;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Transforms;

namespace Pipeline.Desktop.Transforms {
    public class CompiledRegexMatchTransform : BaseTransform, ITransform {
        private readonly Regex _regex;
        private readonly Field[] _input;

        public CompiledRegexMatchTransform(IContext context) : base(context) {
            _input = MultipleInput();
            _regex = new Regex(context.Transform.Pattern, RegexOptions.Compiled);
        }

        public IRow Transform(IRow row) {
            foreach (var field in _input) {
                var match = _regex.Match(row[field].ToString());
                if (!match.Success) continue;
                row[Context.Field] = match.Value;
                break;
            }
            Increment();
            return row;
        }
    }
}