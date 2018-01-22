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
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.Geography {
    public class GeohashEncodeTransform : BaseTransform {

        private readonly Func<IRow, string> _transform;
        private readonly Func<IRow, double> _getLatitude;
        private readonly Func<IRow, double> _getLongitude;

        public GeohashEncodeTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext())
            {
                return;
            }

            var fields = Context.GetAllEntityFields().ToArray();

            if (HasInvalidCoordinate(fields, Context.Operation, Context.Operation.Latitude, "latitude")) {
                return;
            }

            if (HasInvalidCoordinate(fields, Context.Operation, Context.Operation.Longitude, "longitude")) {
                return;
            }

            if (Context.Operation.Length < 1 || Context.Operation.Length > 13) {
                Error("The GeohashEncode method's precision must be between 1 and 13.");
                Run = false;
                return;
            }

            var input = MultipleInput();

            var latField = fields.FirstOrDefault(f => f.Alias.Equals(Context.Operation.Latitude));
            var longField = fields.FirstOrDefault(f => f.Alias.Equals(Context.Operation.Longitude));

            if (latField == null) {
                if (double.TryParse(Context.Operation.Latitude, out double lat)) {
                    _getLatitude = row => lat;
                } else {
                    if (input.Length > 0) {
                        latField = input.First();
                        _getLatitude = row => Convert.ToDouble(row[latField]);
                    } else {
                        Context.Warn($"Trouble determing latitude for geohash method. {context.Operation.Latitude} is not a field or a double value.");
                    }

                }
            } else {
                _getLatitude = row => Convert.ToDouble(row[latField]);
            }

            if (longField == null) {
                if (double.TryParse(Context.Operation.Longitude, out var lon)) {
                    _getLongitude = row => lon;
                } else {
                    if (input.Length > 1) {
                        longField = input[1];
                        _getLongitude = row => Convert.ToDouble(row[longField]);
                    } else {
                        Context.Warn($"Trouble determining longitude for geohash method. {Context.Operation.Longitude} is not a field or a double value.");
                    }
                }
            } else {
                _getLongitude = row => Convert.ToDouble(row[longField]);
            }

            _transform = row => NGeoHash.Portable.GeoHash.Encode(_getLatitude(row), _getLongitude(row), Context.Operation.Length);
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _transform(row);
            Increment();
            return row;
        }

        private bool HasInvalidCoordinate(IEnumerable<Field> fields, Operation t, string valueOrField, string name) {
            if (fields.All(f => f.Alias != valueOrField) && !double.TryParse(valueOrField, out var doubleValue)) {
                Error($"The {t.Method} method's {name} parameter: {valueOrField}, is not a valid field or numeric value.");
                Run = false;
                return true;
            }
            return false;
        }
    }
}
