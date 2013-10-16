using System.Collections.Generic;
using System.Text.RegularExpressions;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {

    public class FromRegexOperation : AbstractOperation {
        private readonly string _inKey;
        private readonly IParameters _parameters;
        private readonly Regex _regex;
        private readonly Dictionary<string, string> _map = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _typeMap = new Dictionary<string, string>();

        public FromRegexOperation(string inKey, string pattern, IParameters parameters) {
            _inKey = inKey;
            _parameters = parameters;
            _regex = new Regex(pattern, RegexOptions.Compiled);

            foreach (var field in parameters) {
                _map[field.Value.Name] = field.Key;
            }

            foreach (var field in parameters) {
                _typeMap[field.Value.Name] = field.Value.SimpleType;
            }

        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                var match = _regex.Match(row[_inKey].ToString());

                if (match.Groups.Count == 0)
                    continue;

                foreach (var pair in _parameters) {
                    var group = match.Groups[pair.Key];
                    if (@group != null) {
                        row[pair.Key] = Common.ConversionMap[pair.Value.SimpleType](@group.Value);
                    }
                }
                yield return row;
            }
        }
    }
}