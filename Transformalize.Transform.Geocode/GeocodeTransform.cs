using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Google.Maps;
using Google.Maps.Geocoding;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Transforms;

namespace Transformalize.Transform.Geocode {

    public class GeocodeTransform : BaseTransform {

        private readonly Field _input;
        private readonly Field[] _output;
        private static RateGate _rateGate;
        private readonly int _originalConnectionLimit;
        private readonly ComponentFilter _componentFilter;

        public GeocodeTransform(IContext context) : base(context, "object") {
            _input = SingleInput();
            _output = MultipleOutput();
            if (context.Transform.Key != string.Empty) {
                GoogleSigned.AssignAllServices(new GoogleSigned(context.Transform.Key));
            }
            _originalConnectionLimit = ServicePointManager.DefaultConnectionLimit;
            ServicePointManager.DefaultConnectionLimit = 255;
            _rateGate = new RateGate(Context.Transform.Limit, TimeSpan.FromMilliseconds(Context.Transform.Time));
            _componentFilter = new ComponentFilter {
                AdministrativeArea = Context.Transform.AdministrativeArea,
                Country = Context.Transform.Country,
                Locality = Context.Transform.Locality,
                PostalCode = Context.Transform.PostalCode,
                Route = Context.Transform.Route
            };
        }

        public override IEnumerable<IRow> Transform(IEnumerable<IRow> rows) {
            foreach (var batch in rows.Partition(Context.Entity.UpdateSize)) {
                var enumerated = batch.ToArray();
                var collected = new ConcurrentBag<IRow>();
                Parallel.ForEach(enumerated, (row) => {
                    _rateGate.WaitToProceed();
                    collected.Add(Transform(row));
                    Increment();
                });
                foreach (var row in collected) {
                    yield return row;
                }
            }
        }

        public override IRow Transform(IRow row) {
            var request = new GeocodingRequest {
                Address = row[_input].ToString(),
                Sensor = false,
                Components = _componentFilter
            };
            try {
                var response = new GeocodingService().GetResponse(request);

                switch (response.Status) {
                    case ServiceResponseStatus.Ok:
                        var first = response.Results.First();
                        foreach (var field in _output) {
                            switch (field.Name.ToLower()) {
                                case "lat":
                                case "latitude":
                                    row[field] = first.Geometry.Location.Latitude;
                                    break;
                                case "lon":
                                case "long":
                                case "longitude":
                                    row[field] = first.Geometry.Location.Longitude;
                                    break;
                                case "address":
                                case "formattedaddress":
                                    if (field.Equals(_input))
                                        break;
                                    row[field] = first.FormattedAddress;
                                    Context.Info(first.FormattedAddress);
                                    break;
                            }
                        }
                        break;
                    default:
                        Context.Error("Error from Google MAPS API: " + response.Status);
                        break;
                }

            } catch (Exception ex) {
                Context.Error(ex.Message);
            }
            Increment();
            return row;
        }

        public override void Dispose() {
            base.Dispose();
            _rateGate.Dispose();
            ServicePointManager.DefaultConnectionLimit = _originalConnectionLimit;
        }
    }
}
