using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class AppendOperation : ShouldRunOperation {
        private readonly string _value;
        private readonly IParameter _parameter;
        private readonly bool _useParameter;

        public AppendOperation(string inKey, string outKey, string value, IParameter parameter)
            : base(inKey, outKey) {
            Name = "Append (" + outKey + ")";

            _value = value;
            _parameter = parameter;
            if (!value.Equals(string.Empty) || parameter == null)
                return;

            if (parameter.HasValue()) {
                _value = parameter.Value.ToString();
            } else {
                _useParameter = true;
            }
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = row[InKey] + (_useParameter ? row[_parameter.Name].ToString() : _value);
                }
                yield return row;
            }
        }
    }
}