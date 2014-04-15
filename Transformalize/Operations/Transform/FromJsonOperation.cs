using System.Collections.Generic;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.fastJSON;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {

    public class FromJsonOperation : ShouldRunOperation {

        private readonly IEnumerable<KeyValuePair<string, IParameter>> _parameters;

        public FromJsonOperation(string inKey, IParameters parameters)
            : base(inKey, string.Empty) {
            _parameters = parameters.ToEnumerable();
            Name = string.Format("FromJsonOperation (in:{0})", inKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var input = row[InKey].ToString();

                    object response;
                    var success = JSON.Instance.TryParse(input, out response);

                    if (success) {
                        var dict = response as Dictionary<string, object>;
                        if (dict != null) {
                            foreach (var pair in _parameters) {
                                var value = dict[pair.Value.Name];
                                if (value is string || value is int || value is long || value is double) {
                                    row[pair.Key] = value;
                                } else {
                                    row[pair.Key] = JSON.Instance.ToJSON(value);
                                }
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