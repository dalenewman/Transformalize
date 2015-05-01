using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main.Parameters;
using Transformalize.Operations.Transform;

namespace Transformalize.Operations.Validate {
    public class EqualsOperation : ShouldRunOperation {
        private readonly IParameter _parameter;

        public EqualsOperation(string key, IParameter parameter)
            : base(parameter.Name, key) {
            _parameter = parameter;
            Name = "Equals (" + InKey + " => " + OutKey + ")";
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = row[OutKey].Equals(_parameter.HasValue() ? _parameter.Value : row[_parameter.Name]);
                } else { Skip(); }
                yield return row;
            }
        }
    }
}