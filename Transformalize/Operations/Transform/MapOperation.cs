using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {

    public class MapOperation : TflOperation {

        private readonly string _outType;
        private readonly Map _endsWith;
        private readonly Map _equals;
        private readonly bool _hasEndsWith;
        private readonly bool _hasEquals;
        private readonly bool _hasStartsWith;
        private readonly Map _startsWith;

        public MapOperation(string inKey, string outKey, string outType, IEnumerable<Map> maps)
            : base(inKey, outKey) {

            var m = maps.ToArray();

            _outType = outType;

            _equals = m[0];
            _hasEquals = _equals.Any();

            _startsWith = m[1];
            _hasStartsWith = _startsWith.Any();

            _endsWith = m[2];
            _hasEndsWith = _endsWith.Any();

            ApplyDataTypes(_equals);
            ApplyDataTypes(_startsWith);
            ApplyDataTypes(_endsWith);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {

                if (ShouldRun(row)) {

                    var found = false;
                    var value = row[InKey].ToString();

                    if (_hasEquals) {
                        if (_equals.ContainsKey(value)) {
                            row[OutKey] = _equals[value].Value ?? row[_equals[value].Parameter];
                            found = true;
                        }
                    }

                    if (!found && _hasStartsWith) {
                        foreach (var pair in _startsWith.Where(pair => value.StartsWith(pair.Key))) {
                            row[OutKey] = pair.Value.Value ?? row[pair.Value.Parameter];
                            found = true;
                            break;
                        }
                    }

                    if (!found && _hasEndsWith) {
                        foreach (var pair in _endsWith.Where(pair => value.EndsWith(pair.Key))) {
                            row[OutKey] = pair.Value.Value ?? row[pair.Value.Parameter];
                            found = true;
                            break;
                        }
                    }

                    if (!found && _equals.ContainsKey("*")) {
                        row[OutKey] = _equals["*"].Value ?? row[_equals["*"].Parameter];
                        found = true;
                    }

                    if (!found) {
                        row[OutKey] = row[InKey];
                    }

                }

                yield return row;
            }
        }

        public void ApplyDataTypes(Map map) {
            if (!map.Any())
                return;

            foreach (var pair in _equals.Where(pair => pair.Value.Value != null)) {
                _equals[pair.Key].Value = Common.ObjectConversionMap[_outType](pair.Value.Value);
            }
        }
    }
}