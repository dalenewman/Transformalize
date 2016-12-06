using System.Collections.Generic;
using System.Linq;
using Google.Maps;
using Google.Maps.Geocoding;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Transform.Geocode {
    public class GeocodeTransform : BaseTransform {
        private readonly Field _input;
        private readonly Field[] _output;
        private readonly HashSet<ServiceResponseStatus> _statuses = new HashSet<ServiceResponseStatus>();

        public GeocodeTransform(IContext context) : base(context, "object")
        {
            _input = SingleInput();
            _output = MultipleOutput();
            if (context.Transform.Key != string.Empty) {
                GoogleSigned.AssignAllServices(new GoogleSigned(context.Transform.Key));
            }
        }

        public override IRow Transform(IRow row) {
            var request = new GeocodingRequest {
                Address = row[_input].ToString(),
                Sensor = false
            };

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
                                row[field] = first.FormattedAddress;
                                break;
                        }
                    }
                    break;
                default:
                    if (_statuses.Add(response.Status)) {
                        Context.Error("Error from Google MAPS API, " + response.Status);
                    }
                    break;
            }

            Increment();
            return row;
        }
    }
}
