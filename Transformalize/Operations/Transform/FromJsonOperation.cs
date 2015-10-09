using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cfg.Net.Parsers.fastJSON;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;
using Transformalize.Main.Parameters;

namespace Transformalize.Operations.Transform {

    public class FromJsonOperation : ShouldRunOperation {

        private readonly KeyValuePair<string, IParameter>[] _parameters;

        public FromJsonOperation(string inKey, IParameters parameters)
            : base(inKey, string.Empty) {
            _parameters = parameters.ToEnumerable().ToArray();
            Name = $"FromJsonOperation (in:{inKey})";
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var input = row[InKey].ToString();
                    var parsed = new object();
                    bool success;

                    try {
                        parsed = JSON.Parse(input);
                        success = true;
                    } catch (Exception ex) {
                        success = false;
                    }

                    if (success) {
                        var dict = parsed as Dictionary<string, object>;
                        if (dict != null) {
                            foreach (var pair in _parameters) {
                                var value = dict[pair.Value.Name];
                                if (value is string || value is int || value is long || value is double) {
                                    row[pair.Key] = value;
                                } else {
                                    row[pair.Key] = JsonConvert.SerializeObject(value);
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