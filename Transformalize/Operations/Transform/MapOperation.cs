using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {

    public class MapOperation : ShouldRunOperation {

        private readonly string _outType;
        private readonly Map _endsWith;
        private readonly Map _equals;
        private readonly bool _hasEndsWith;
        private readonly bool _hasEquals;
        private readonly bool _hasStartsWith;
        private readonly Map _startsWith;
        private readonly Dictionary<string, Func<object, object>> _conversionMap = Common.GetObjectConversionMap();

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

            Name = string.Format("MapOperation ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {

                if (ShouldRun(row)) {

                    var found = false;
                    var value = row[InKey].ToString();

                    if (_hasEquals) {
                        if (_equals.ContainsKey(value)) {
                            row[OutKey] = _equals[value].UseParameter ? row[_equals[value].Parameter] : _equals[value].Value;
                            found = true;
                        }
                    }

                    if (!found && _hasStartsWith) {
                        foreach (var pair in _startsWith.Where(pair => value.StartsWith(pair.Key))) {
                            row[OutKey] = pair.Value.UseParameter ? row[pair.Value.Parameter] : pair.Value.Value;
                            found = true;
                            break;
                        }
                    }

                    if (!found && _hasEndsWith) {
                        foreach (var pair in _endsWith.Where(pair => value.EndsWith(pair.Key))) {
                            row[OutKey] = pair.Value.UseParameter ? row[pair.Value.Parameter] : pair.Value.Value;
                            found = true;
                            break;
                        }
                    }

                    if (!found && _equals.ContainsKey("*")) {
                        row[OutKey] = _equals["*"].UseParameter ? row[_equals["*"].Parameter] : _equals["*"].Value;
                        found = true;
                    }

                    if (!found) {
                        row[OutKey] = row[InKey];
                    }
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }

        public void ApplyDataTypes(Map map) {
            if (!map.Any())
                return;

            foreach (var pair in _equals.Where(pair => pair.Value.Value != null)) {
                _equals[pair.Key].Value = _conversionMap[_outType](pair.Value.Value);
            }
        }
    }
}