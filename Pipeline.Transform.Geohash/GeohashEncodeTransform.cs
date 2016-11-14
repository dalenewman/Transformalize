#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Transforms;

namespace Pipeline.Transform.Geohash {
    public class GeohashEncodeTransform : BaseTransform {

        private readonly Field _latField;
        private readonly double _lat;
        private readonly Field _longField;
        private readonly double _lon;
        private readonly Func<IRow, string> _transform;
        private readonly Func<IRow, double> _getLatitude;
        private readonly Func<IRow, double> _getLongitude;

        public GeohashEncodeTransform(IContext context) : base(context, "string") {

            var fields = context.GetAllEntityFields().ToArray();
            var input = MultipleInput();

            _latField = fields.FirstOrDefault(f => f.Alias.Equals(context.Transform.Latitude));
            _longField = fields.FirstOrDefault(f => f.Alias.Equals(context.Transform.Longitude));

            if (_latField == null) {
                if (double.TryParse(context.Transform.Latitude, out _lat)) {
                    _getLatitude = row => _lat;
                } else {
                    if (input.Length > 0) {
                        _latField = input.First();
                        _getLatitude = row => Convert.ToDouble(row[_latField]);
                    } else {
                        context.Warn($"Trouble determing latitude for geohash method. {context.Transform.Latitude} is not a field or a double value.");
                    }

                }
            } else {
                _getLatitude = row => Convert.ToDouble(row[_latField]);
            }

            if (_longField == null) {
                if (double.TryParse(context.Transform.Longitude, out _lon)) {
                    _getLongitude = row => _lon;
                } else {
                    if (input.Length > 1) {
                        _longField = input[1];
                        _getLongitude = row => Convert.ToDouble(row[_longField]);
                    } else {
                        context.Warn($"Trouble determining longitude for geohash method. {context.Transform.Longitude} is not a field or a double value.");
                    }
                }
            } else {
                _getLongitude = row => Convert.ToDouble(row[_longField]);
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
