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
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.Geography {
    public class GeohashEncodeTransform : BaseTransform {

        private readonly Func<IRow, string> _transform;
        private readonly Func<IRow, double> _getLatitude;
        private readonly Func<IRow, double> _getLongitude;

        public GeohashEncodeTransform(IContext context) : base(context, "string") {
            double lon;
            double lat;
            var fields = context.GetAllEntityFields().ToArray();

            if (HasInvalidCoordinate(fields, context.Operation, context.Operation.Latitude, "latitude")) {
                return;
            }

            if (HasInvalidCoordinate(fields, context.Operation, context.Operation.Longitude, "longitude")) {
                return;
            }

            if (context.Operation.Length < 1 || context.Operation.Length > 13) {
                Error("The GeohashEncode method's precision must be between 1 and 13.");
                Run = false;
                return;
            }

            var input = MultipleInput();

            var latField = fields.FirstOrDefault(f => f.Alias.Equals(context.Operation.Latitude));
            var longField = fields.FirstOrDefault(f => f.Alias.Equals(context.Operation.Longitude));

            if (latField == null) {
                if (double.TryParse(context.Operation.Latitude, out lat)) {
                    _getLatitude = row => lat;
                } else {
                    if (input.Length > 0) {
                        latField = input.First();
                        _getLatitude = row => Convert.ToDouble(row[latField]);
                    } else {
                        context.Warn($"Trouble determing latitude for geohash method. {context.Operation.Latitude} is not a field or a double value.");
                    }

                }
            } else {
                _getLatitude = row => Convert.ToDouble(row[latField]);
            }

            if (longField == null) {
                if (double.TryParse(context.Operation.Longitude, out lon)) {
                    _getLongitude = row => lon;
                } else {
                    if (input.Length > 1) {
                        longField = input[1];
                        _getLongitude = row => Convert.ToDouble(row[longField]);
                    } else {
                        context.Warn($"Trouble determining longitude for geohash method. {context.Operation.Longitude} is not a field or a double value.");
                    }
                }
            } else {
                _getLongitude = row => Convert.ToDouble(row[longField]);
            }

            _transform = row => NGeoHash.Portable.GeoHash.Encode(_getLatitude(row), _getLongitude(row), context.Operation.Length);
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _transform(row);
            Increment();
            return row;
        }

        private bool HasInvalidCoordinate(Field[] fields, Operation t, string valueOrField, string name) {
            double doubleValue;
            if (fields.All(f => f.Alias != valueOrField) && !double.TryParse(valueOrField, out doubleValue)) {
                Error($"The {t.Method} method's {name} parameter: {valueOrField}, is not a valid field or numeric value.");
                Run = false;
                return true;
            }
            return false;
        }
    }
}
