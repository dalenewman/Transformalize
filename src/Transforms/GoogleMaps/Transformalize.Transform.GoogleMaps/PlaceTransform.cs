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
using Google.Maps.Places;
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

   public class PlaceTransform : BaseTransform {

      private readonly Field _input;
      private readonly Field[] _output;
      private static RateGate _rateGate;
      private readonly int _originalConnectionLimit;

      public PlaceTransform(IContext context = null) : base(context, "object") {

         ProducesFields = true;

         if (IsMissingContext()) {
            return;
         }

         if (IsMissing(Context.Operation.ApiKey)) {
            return;
         }

         if (Context.Operation.Parameters.Any()) {

            var place = Context.Operation.Parameters.FirstOrDefault(p => p.Name.ToLower().In("place", "placeid"));
            if (place == null) {
               Error("The google-place transform requires an output field named 'place' or 'placeid'.");
               Run = false;
               return;
            }

            var lat = Context.Operation.Parameters.FirstOrDefault(p => p.Name.ToLower().In("lat", "latitude"));
            if (lat != null) {
               if (lat.Type != "double") {
                  Error($"The google-place {lat.Name} field must be of type double.");
                  Run = false;
                  return;
               }
            }

            var lon = Context.Operation.Parameters.FirstOrDefault(p => p.Name.ToLower().In("lon", "long", "longitude"));
            if (lon != null) {
               if (lon.Type != "double") {
                  Error($"The google-place {lon.Name} field must be of type double.");
                  Run = false;
                  return;
               }
            }

         } else {
            Error("The google-place transform requires a collection of output fields; namely: placeid, latitude (optional), longitude (optional), and formattedaddress (optional).");
            Run = false;
            return;
         }


         if (Run) {
            Context.Debug(() => "PlaceTransform (google-place) is initialized and will run.");
         } else {
            Context.Debug(() => "PlaceTransform (google-place) will not run due to setup issues.");
         }

         _input = SingleInputForMultipleOutput();
         _output = MultipleOutput();

         GoogleSigned.AssignAllServices(new GoogleSigned(Context.Operation.ApiKey));

         _originalConnectionLimit = ServicePointManager.DefaultConnectionLimit;
         ServicePointManager.DefaultConnectionLimit = 255;
         _rateGate = new RateGate(Context.Operation.Limit, TimeSpan.FromMilliseconds(Context.Operation.Time));

      }

      public override IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {
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

      public override IRow Operate(IRow row) {

         var request = new TextSearchRequest { Query = row[_input].ToString() };

         try {
            var response = new PlacesService().GetResponse(request);

            switch (response.Status) {
               case ServiceResponseStatus.Ok:
                  var first = response.Results.First();

                  foreach (var field in _output) {
                     switch (field.Name.ToLower()) {
                        case "rating":
                           row[field] = first.Rating;
                           break;
                        case "name":
                           row[field] = first.Name;
                           break;
                        case "type":
                        case "locationtype":
                           row[field] = first.Geometry.LocationType.ToString();
                           break;
                        case "icon":
                           row[field] = first.Icon;
                           break;
                        case "place":
                        case "placeid":
                           row[field] = first.PlaceId;
                           break;
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
         yield return new OperationSignature("google-place") { Parameters = new List<OperationParameter>(1) { new OperationParameter("apikey") } };
      }

      public override void Dispose() {
         base.Dispose();
         _rateGate?.Dispose();
         if (_originalConnectionLimit > 0) {
            ServicePointManager.DefaultConnectionLimit = _originalConnectionLimit;
         }

      }
   }
}
