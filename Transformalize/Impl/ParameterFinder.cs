using System.Collections.Generic;
using System.Text.RegularExpressions;
using Transformalize.Contracts;

namespace Transformalize.Impl {
    public class ParameterFinder : IParameterFinder {
        private readonly char _prefix;
        private readonly Regex _regex;

        public ParameterFinder(char prefix = '@') {
            _prefix = prefix;
#if NETS10
            _regex = new Regex($"\\{prefix}([\\w.$]+)", RegexOptions.CultureInvariant);
#else
        _regex = new Regex($"\\{prefix}([\\w.$]+)", RegexOptions.CultureInvariant | RegexOptions.Compiled);
#endif
        }

        public IEnumerable<string> Find(string query) {
            var matches = _regex.Matches(query);
            for (var i = 0; i < matches.Count; i++) {
                var match = matches[i];
                var parameter = match.Value.TrimStart(new[] { _prefix });
                yield return parameter;
            }
        }
    }
}