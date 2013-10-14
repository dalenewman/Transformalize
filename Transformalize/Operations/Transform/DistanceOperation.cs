using System;
using System.Collections.Generic;
using System.Device.Location;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations.Transform
{
    public class DistanceOperation : AbstractOperation
    {
        private readonly string _outKey;
        private readonly string _units;
        private readonly IParameters _parameters;
        private readonly int _paramCount;
        private readonly SortedDictionary<string, Tuple<string, object>> _params = new SortedDictionary<string, Tuple<string, object>>();
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public DistanceOperation(string outKey, string units, IParameters parameters)
        {
            _outKey = outKey;
            _units = units;
            _parameters = parameters;

            _paramCount = _parameters.Count;
            if (_paramCount < 4) {
                _log.Error("You have a distance transform with less than 4 parameters.  It needs fromLat, fromLong, toLat, toLong.");
                Environment.Exit(1);
            }
            _params["fromLat"] = new Tuple<string, object>(_parameters[0].Name, _parameters[0].Value);
            _params["fromLong"] = new Tuple<string, object>(_parameters[1].Name, _parameters[1].Value);
            _params["toLat"] = new Tuple<string, object>(_parameters[2].Name, _parameters[2].Value);
            _params["toLong"] = new Tuple<string, object>(_parameters[3].Name, _parameters[3].Value);

        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            foreach (var row in rows)
            {
                var fromLat = Convert.ToDouble(row[_params["fromLat"].Item1] ?? _params["fromLat"].Item2);
                var fromLong = Convert.ToDouble(row[_params["fromLong"].Item1] ?? _params["fromLong"].Item2);
                var toLat = Convert.ToDouble(row[_params["toLat"].Item1] ?? _params["toLat"].Item2);
                var toLong = Convert.ToDouble(row[_params["toLong"].Item1] ?? _params["toLong"].Item2);

                var meters = new GeoCoordinate(fromLat, fromLong).GetDistanceTo(new GeoCoordinate(toLat, toLong));
                switch (_units) {
                    case "miles":
                        row[_outKey] = 0.000621371 * meters;
                        break;
                    case "kilometers":
                        row[_outKey] = 0.001 * meters;
                        break;
                    default:
                        row[_outKey] = meters;
                        break;
                }

                yield return row;
            }
        }
    }
}