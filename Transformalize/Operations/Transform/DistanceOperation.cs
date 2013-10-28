using System;
using System.Collections.Generic;
using System.Device.Location;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class DistanceOperation : AbstractOperation {
        private readonly string _outKey;
        private readonly string _units;
        private readonly SortedDictionary<string, Tuple<string, object>> _params = new SortedDictionary<string, Tuple<string, object>>();

        private readonly Dictionary<string, Func<double, double>> _conversion = new Dictionary<string, Func<double, double>>() {
            {"meters",(x => x)},
            {"kilometers",(x => 0.001 * x)},
            {"miles",(x => 0.000621371 * x)},
        };

        public DistanceOperation(string outKey, string units, IParameter fromLat, IParameter fromLong, IParameter toLat, IParameter toLong) {

            if (!_conversion.ContainsKey(units)) {
                Error("Error in Distance transform. I do not recognize {0} units.  Try meters, kilometers, or miles.", units);
                Environment.Exit(1);
            }

            _outKey = outKey;
            _units = units;

            _params["fromLat"] = new Tuple<string, object>(fromLat.Name, fromLat.Value);
            _params["fromLong"] = new Tuple<string, object>(fromLong.Name, fromLong.Value);
            _params["toLat"] = new Tuple<string, object>(toLat.Name, toLat.Value);
            _params["toLong"] = new Tuple<string, object>(toLong.Name, toLong.Value);

        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                var fromLat = Convert.ToDouble(row[_params["fromLat"].Item1] ?? _params["fromLat"].Item2);
                var fromLong = Convert.ToDouble(row[_params["fromLong"].Item1] ?? _params["fromLong"].Item2);
                var toLat = Convert.ToDouble(row[_params["toLat"].Item1] ?? _params["toLat"].Item2);
                var toLong = Convert.ToDouble(row[_params["toLong"].Item1] ?? _params["toLong"].Item2);

                var meters = new GeoCoordinate(fromLat, fromLong).GetDistanceTo(new GeoCoordinate(toLat, toLong));
                row[_outKey] = _conversion[_units](meters);
                yield return row;
            }
        }
    }
}