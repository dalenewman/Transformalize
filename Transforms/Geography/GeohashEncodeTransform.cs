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
using System.Linq;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Transform.Geography {
    public class GeohashEncodeTransform : BaseTransform {
        private readonly Func<IRow, string> _transform;
        private readonly Func<IRow, double> _getLatitude;
        private readonly Func<IRow, double> _getLongitude;

        public GeohashEncodeTransform(IContext context) : base(context, "string") {
            double lon;
            double lat;
            var fields = context.GetAllEntityFields().ToArray();
            var input = MultipleInput();

            var latField = fields.FirstOrDefault(f => f.Alias.Equals(context.Transform.Latitude));
            var longField = fields.FirstOrDefault(f => f.Alias.Equals(context.Transform.Longitude));

            if (latField == null) {
                if (double.TryParse(context.Transform.Latitude, out lat)) {
                    _getLatitude = row => lat;
                } else {
                    if (input.Length > 0) {
                        latField = input.First();
                        _getLatitude = row => Convert.ToDouble(row[latField]);
                    } else {
                        context.Warn($"Trouble determing latitude for geohash method. {context.Transform.Latitude} is not a field or a double value.");
                    }

                }
            } else {
                _getLatitude = row => Convert.ToDouble(row[latField]);
            }

            if (longField == null) {
                if (double.TryParse(context.Transform.Longitude, out lon)) {
                    _getLongitude = row => lon;
                } else {
                    if (input.Length > 1) {
                        longField = input[1];
                        _getLongitude = row => Convert.ToDouble(row[longField]);
                    } else {
                        context.Warn($"Trouble determining longitude for geohash method. {context.Transform.Longitude} is not a field or a double value.");
                    }
                }
            } else {
                _getLongitude = row => Convert.ToDouble(row[longField]);
            }

            _transform = row => NGeoHash.Portable.GeoHash.Encode(_getLatitude(row), _getLongitude(row), context.Transform.Length);
        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = _transform(row);
            Increment();
            return row;
        }
    }
}
