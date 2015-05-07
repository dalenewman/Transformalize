using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;
using Transformalize.Main.Parameters;
using Transformalize.Operations.Transform;

namespace Transformalize.Operations.Validate {
    public class EqualsOperation : ShouldRunOperation {
        private readonly IList<KeyValuePair<string, IParameter>> _parameters;

        private readonly Func<Row, string, IList<KeyValuePair<string, IParameter>>, bool> _operation;

        public EqualsOperation(string key, IParameters parameters)
            : base(parameters.Count == 1 ? parameters[0].Name : "*", key) {
            _parameters = parameters.ToEnumerable().ToList();
            Name = "Equals(" + InKey + "=>" + OutKey + ")";

            if (InKey == "*") {
                _operation = CompareMultiple;
            } else {
                _operation = CompareSingle;
            }

        }

        private static bool CompareMultiple(Row row, string outKey, IList<KeyValuePair<string, IParameter>> parameters) {
            var values = parameters.Select(p => p.Value.HasValue() ? p.Value.Value : row[p.Key]).ToArray();
            var first = values.First();
            return values.Skip(1).All(v => v.Equals(first));
        }

        private static bool CompareSingle(Row row, string outKey, IList<KeyValuePair<string, IParameter>> parameters) {
            return row[outKey].Equals(parameters[0].Value.HasValue() ? parameters[0].Value.Value : row[parameters[0].Key]);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = _operation(row, OutKey, _parameters);
                } else { Skip(); }
                yield return row;
            }
        }


    }
}