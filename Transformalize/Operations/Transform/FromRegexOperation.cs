using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {

    public class FromRegexOperation : ShouldRunOperation {

        private readonly IParameters _parameters;
        private readonly Regex _regex;
        private readonly Dictionary<string, string> _map = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _typeMap = new Dictionary<string, string>();

        public FromRegexOperation(string inKey, string pattern, IParameters parameters)
            : base(inKey, string.Empty) {
            _parameters = parameters;
            _regex = new Regex(pattern, RegexOptions.Compiled);

            foreach (var field in parameters) {
                _map[field.Value.Name] = field.Key;
            }

            foreach (var field in parameters) {
                _typeMap[field.Value.Name] = field.Value.SimpleType;
            }
            Name = string.Format("FromRegexOperation (in:{0})", inKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var match = _regex.Match(row[InKey].ToString());

                    if (match.Groups.Count == 0)
                        continue;

                    foreach (var pair in _parameters) {
                        var group = match.Groups[pair.Key];
                        if (@group != null) {
                            try {
                                row[pair.Key] = Common.ConversionMap[pair.Value.SimpleType](@group.Value);
                            } catch (Exception ex) {
                                Logger.EntityWarn(EntityName, "Trouble converting '{0}' to '{1}' type.  Matched from {2}. {3}", row[pair.Key], pair.Value.SimpleType, row[InKey], ex.Message);
                            }
                        }
                    }
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }
    }
}