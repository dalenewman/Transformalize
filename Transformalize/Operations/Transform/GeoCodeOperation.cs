using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.GoogleMaps.LocationServices;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Logging;
using Transformalize.Main;
using Transformalize.Main.Parameters;

namespace Transformalize.Operations.Transform {

    public class GeoCodeOperation : ShouldRunOperation {
        private readonly int _sleep;
        private readonly IParameter[] _parameters;
        private readonly ILocationService _service;
        private readonly bool _useParameters;

        public GeoCodeOperation(string inKey, string outKey, int sleep, bool useHttps, IParameters parameters)
            : base(inKey, outKey) {
            _sleep = sleep;
            _parameters = parameters.ToEnumerable().Select(kv => kv.Value).ToArray();
            _service = new GoogleLocationService(useHttps);
            _useParameters = parameters.Count > 1;
            Name = string.Format("GeoCode ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var r = row;
                    var address = string.Empty;
                    try {
                        address = _useParameters ? string.Join(" ", _parameters.Select(p => r[p.Name] ?? p.Value)) : row[InKey].ToString();
                        var latLong = _service.GetLatLongFromAddress(address);
                        row[OutKey] = string.Format("{0},{1}", latLong.Latitude, latLong.Longitude);
                    } catch (Exception e) {
                        row[OutKey] = "0,0";
                        TflLogger.Warn(ProcessName, EntityName, "GeoCoding failed for {0}. {1}", address, e.Message);
                    }
                    if (_sleep > 0) {
                        TflLogger.Info(ProcessName, EntityName, "GeoCoded {0} to {1}.", address, row[OutKey]);
                        System.Threading.Thread.Sleep(_sleep);
                    }
                }
                yield return row;
            }
        }
    }
}