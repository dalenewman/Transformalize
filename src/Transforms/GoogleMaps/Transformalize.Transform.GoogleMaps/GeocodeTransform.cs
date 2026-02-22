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

using Google.Maps;
using Google.Maps.Geocoding;
using Google.Maps.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Transforms;

namespace Transformalize.Transform.GoogleMaps {

   public class GeocodeTransform : BaseTransform {

      private readonly Field _input;
      private readonly Field[] _output;
      private static RateGate _rateGate;
      private readonly int _originalConnectionLimit;
      private readonly ComponentFilter _componentFilter;
      private readonly GeocodingService _service;


      public GeocodeTransform(IContext context = null) : base(context, "object") {

         ProducesFields = true;

         if (IsMissingContext()) {
            return;
         }

         if (IsMissing(Context.Operation.ApiKey)) {
            return;
         }

         if (Context.Operation.Parameters.Any()) {

            var lat = Context.Operation.Parameters.FirstOrDefault(p => p.Name.ToLower().In("lat", "latitude"));
            if (lat == null) {
               Error("The google-geocode transform requires an output field named lat, or latitude.");
               Run = false;
               return;
            }
            if (lat.Type != "double") {
               Error($"The google-geocode {lat.Name} field must be of type double.");
               Run = false;
               return;
            }

            var lon = Context.Operation.Parameters.FirstOrDefault(p => p.Name.ToLower().In("lon", "long", "longitude"));
            if (lon == null) {
               Error("The google-geocode (geocode) transform requires an output field named lon, long, or longitude.");
               Run = false;
               return;
            }
            if (lon.Type != "double") {
               Error($"The google-geocode {lon.Name} field must be of type double.");
               Run = false;
               return;
            }
         } else {
            Error("The google-geocode transform requires a collection of output fields; namely: latitude, longitude, and formattedaddress (optional).");
            Run = false;
            return;
         }

         _input = SingleInputForMultipleOutput();
         _output = MultipleOutput();

         GoogleSigned.AssignAllServices(new GoogleSigned(Context.Operation.ApiKey));

         _originalConnectionLimit = ServicePointManager.DefaultConnectionLimit;
         ServicePointManager.DefaultConnectionLimit = 255;
         _rateGate = new RateGate(Context.Operation.Limit, TimeSpan.FromMilliseconds(Context.Operation.Time));
         _componentFilter = new ComponentFilter {
            AdministrativeArea = Context.Operation.AdministrativeArea,
            Country = Context.Operation.Country,
            Locality = Context.Operation.Locality,
            PostalCode = Context.Operation.PostalCode,
            Route = Context.Operation.Route
         };

         try {
            _service = new GeocodingService();
         } catch (Exception ex) {
            Run = false;
            Context.Error("Could not construct GeocodingService. {0}", ex.Message);
         }

         if (Run) {
            Context.Debug(() => "GeocodeTransform (google-geocode) is initialized and will run.");
         } else {
            Context.Debug(() => "GeocodeTransform (google-geocode) will not run due to setup issues.");
         }

      }

      public override IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {
         if (Run) {
            foreach (var batch in rows.Partition(Context.Entity.UpdateSize)) {
               var enumerated = batch.ToArray();
               var collected = new ConcurrentBag<IRow>();
               Parallel.ForEach(enumerated, (row) => {
                  _rateGate.WaitToProceed();
                  collected.Add(Operate(row));
               });
               foreach (var row in collected) {
                  yield return row;
               }
            }
         }
      }

      public override IRow Operate(IRow row) {

         var query = row[_input].ToString();
         var request = new GeocodingRequest { Components = _componentFilter };
         if (query.Contains(" ")) {
            request.Address = query;
         } else {
            request.PlaceId = query;
         }

         try {
            var response = _service.GetResponse(request);

            switch (response.Status) {
               case ServiceResponseStatus.Ok:
                  var first = response.Results.First();
                  foreach (var field in _output) {

                     switch (field.Name.ToLower()) {
                        case "route":
                           var route = first.AddressComponents.FirstOrDefault(ac => ac.Types.Any(t => t.Equals(AddressType.Route)));
                           if (route != null) {
                              row[field] = route.ShortName;
                           }
                           break;
                        case "streetnumber":
                           var streetNumber = first.AddressComponents.FirstOrDefault(ac => ac.Types.Any(t => t.Equals(AddressType.StreetNumber)));
                           if (streetNumber != null) {
                              row[field] = streetNumber.ShortName;
                           }
                           break;
                        case "streetaddress":
                           var streetAddress = first.AddressComponents.FirstOrDefault(ac => ac.Types.Any(t => t.Equals(AddressType.StreetAddress)));
                           if (streetAddress != null) {
                              row[field] = streetAddress.ShortName;
                           }
                           break;
                        case "premise":
                           var premise = first.AddressComponents.FirstOrDefault(ac => ac.Types.Any(t => t.Equals(AddressType.Premise)));
                           if (premise != null) {
                              row[field] = premise.ShortName;
                           }
                           break;
                        case "subpremise":
                           var subPremise = first.AddressComponents.FirstOrDefault(ac => ac.Types.Any(t => t.Equals(AddressType.Subpremise)));
                           if (subPremise != null) {
                              row[field] = subPremise.ShortName;
                           }
                           break;
                        case "floor":
                           var floor = first.AddressComponents.FirstOrDefault(ac => ac.Types.Any(t => t.Equals(AddressType.Floor)));
                           if (floor != null) {
                              row[field] = floor.ShortName;
                           }
                           break;
                        case "room":
                           var room = first.AddressComponents.FirstOrDefault(ac => ac.Types.Any(t => t.Equals(AddressType.Room)));
                           if (room != null) {
                              row[field] = room.ShortName;
                           }
                           break;
                        case "neighborhood":
                           var neighborhood = first.AddressComponents.FirstOrDefault(ac => ac.Types.Any(t => t.Equals(AddressType.Neighborhood)));
                           if (neighborhood != null) {
                              row[field] = neighborhood.ShortName;
                           }
                           break;
                        case "partialmatch":
                           row[field] = first.PartialMatch;
                           break;
                        case "lat":
                        case "latitude":
                           row[field] = first.Geometry.Location.Latitude;
                           break;
                        case "type":
                        case "locationtype":
                           row[field] = first.Geometry.LocationType.ToString();
                           break;
                        case "place":
                        case "placeid":
                           row[field] = first.PlaceId;
                           break;
                        case "locality":
                           var locality = first.AddressComponents.FirstOrDefault(ac => ac.Types.Any(t => t.Equals(AddressType.Locality)));
                           if (locality != null) {
                              row[field] = locality.ShortName;
                           }
                           break;
                        case "political":
                           var political = first.AddressComponents.FirstOrDefault(ac => ac.Types.Any(t => t.Equals(AddressType.Political)));
                           if (political != null) {
                              row[field] = political.ShortName;
                           }
                           break;
                        case "state":
                        case "administrative_area_level_1":
                           var state = first.AddressComponents.FirstOrDefault(ac => ac.Types.Any(t => t.Equals(AddressType.AdministrativeAreaLevel1)));
                           if (state != null) {
                              row[field] = state.ShortName;
                           }
                           break;
                        case "county":
                        case "administrative_area_level_2":
                           var county = first.AddressComponents.FirstOrDefault(ac => ac.Types.Any(t => t.Equals(AddressType.AdministrativeAreaLevel2)));
                           if (county != null) {
                              row[field] = county.ShortName;
                           }
                           break;
                        case "country":
                           var country = first.AddressComponents.FirstOrDefault(ac => ac.Types.Any(t => t.Equals(AddressType.Country)));
                           if (country != null) {
                              row[field] = country.ShortName;
                           }
                           break;
                        case "zip":
                        case "zipcode":
                        case "postalcode":
                           var zip = first.AddressComponents.FirstOrDefault(ac => ac.Types.Any(t => t.Equals(AddressType.PostalCode)));
                           if (zip != null) {
                              row[field] = zip.ShortName;
                           }
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
                           Context.Debug(() => first.FormattedAddress);
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

         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("google-geocode") { Parameters = new List<OperationParameter>(1) { new OperationParameter("apikey") } };
         yield return new OperationSignature("fromaddress") { Parameters = new List<OperationParameter>(1) { new OperationParameter("apikey") } };
      }

      public override void Dispose() {
         base.Dispose();
         _rateGate?.Dispose();
         _service?.Dispose();
         if (_originalConnectionLimit > 0) {
            ServicePointManager.DefaultConnectionLimit = _originalConnectionLimit;
         }
      }
   }
}
