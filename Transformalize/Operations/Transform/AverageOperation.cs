using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class AverageOperation : ShouldRunOperation {
        private readonly string _outType;
        private readonly IParameters _parameters;
        private readonly bool _typeSafe;

        public AverageOperation(string outKey, string outType, IParameters parameters)
            : base(string.Empty, outKey) {
            _outType = outType;
            _parameters = parameters;
            _typeSafe = parameters.ToEnumerable().All(kv => kv.Value.SimpleType.Equals(outType));
            Name = string.Format("Average ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            if (_typeSafe) {
                foreach (var row in rows) {
                    if (ShouldRun(row)) {
                        var items = new List<object>();
                        foreach (var p in _parameters) {
                            items.Add((row[p.Key] ?? p.Value));
                        }
                        row[OutKey] = SwitchType(items);
                        yield return row;
                    }
                }
            } else {
                foreach (var row in rows) {
                    if (ShouldRun(row)) {
                        CalculateAverage(row);
                    }
                    yield return row;
                }
            }
        }

        private void CalculateAverage(Row row) {
            var items = new List<object>();
            byte any = 0;
            var convert = TypeDescriptor.GetConverter(Common.ToSystemType(_outType));
            foreach (var p in _parameters) {
                var possible = row[p.Key] ?? p.Value.Value;
                try {
                    items.Add(convert.ConvertFrom(possible));
                    any = 1;
                } catch (Exception) {
                    continue;  //do not include in average
                }
            }
            row[OutKey] = any > 0 ? SwitchType(items) : 0;
        }



        private object SwitchType(IEnumerable<object> items) {
            switch (_outType) {
                case "decimal":
                    return items.Average(x => Convert.ToDecimal(x));
                case "double":
                    return items.Average(x => Convert.ToDouble(x));
                case "float":
                    return items.Average(x => Convert.ToSingle(x));
                case "single":
                    return items.Average(x => Convert.ToSingle(x));
                case "int32":
                    return items.Average(x => Convert.ToInt32(x));
                case "int64":
                    return items.Average(x => Convert.ToInt64(x));
                default:
                    Warn("Unable to handle average of type {0}", _outType);
                    return 0;
            }
        }
    }
}