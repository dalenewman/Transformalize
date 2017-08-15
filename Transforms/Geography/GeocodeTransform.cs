#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
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

namespace Transformalize.Transform.Geography {

    public class GeocodeTransform : BaseTransform {

        private readonly Field _input;
        private readonly Field[] _output;
        private static RateGate _rateGate;
        private readonly int _originalConnectionLimit;
        private readonly ComponentFilter _componentFilter;

        public GeocodeTransform(IContext context) : base(context, "object") {
            if (IsNotReceiving("string")) {
                return;
            }

            if (context.Transform.Parameters.Any()) {

                var lat = context.Transform.Parameters.FirstOrDefault(p => p.Name.ToLower().In("lat", "latitude"));
                if (lat == null) {
                    Error("The fromaddress (geocode) transform requires an output field named lat, or latitude.");
                    Run = false;
                    return;
                }
                if (lat.Type != "double") {
                    Error($"The goecode {lat.Name} field must be of type double.");
                    Run = false;
                    return;
                }

                var lon = context.Transform.Parameters.FirstOrDefault(p => p.Name.ToLower().In("lon", "long", "longitude"));
                if (lon == null) {
                    Error("The fromaddress (geocode) transform requires an output field named lon, long, or longitude.");
                    Run = false;
                    return;
                }
                if (lon.Type != "double") {
                    Error($"The goecode {lon.Name} field must be of type double.");
                    Run = false;
                    return;
                }
            } else {
                Error("The fromaddress (geocode) transform requires a collection of output fields; namely: latitude, longitude, and formattedaddress (optional).");
                Run = false;
                return;
            }

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
