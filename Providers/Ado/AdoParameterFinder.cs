using System.Collections.Generic;
using System.Text.RegularExpressions;
using Transformalize.Contracts;

namespace Transformalize.Providers.Ado {
    public class AdoParameterFinder : IParameterFinder {

        public static Regex Regex = new Regex("\\@([\\w.$]+)", RegexOptions.CultureInvariant | RegexOptions.Compiled);

        public IEnumerable<string> Find(string query) {
            var matches = Regex.Matches(query);
            for (var i = 0; i < matches.Count; i++) {
                var match = matches[i];
                var parameter = match.Value.TrimStart(new[] { '@' });
                yield return parameter;
            }
        }
    }
}
