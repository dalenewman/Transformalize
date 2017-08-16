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
    public class DistanceTransform : BaseTransform {
        private readonly Field[] _fields;
        private readonly Func<IRow, double> _getDistance;

        public DistanceTransform(IContext context) : base(context, "double") {

            _fields = context.GetAllEntityFields().ToArray();

            if (HasInvalidCoordinate(_fields, context.Transform, context.Transform.FromLat, "from-lat")) {
                return;
            }
            if (HasInvalidCoordinate(_fields, context.Transform, context.Transform.FromLon, "from-lon")) {
                return;
            }
            if (HasInvalidCoordinate(_fields, context.Transform, context.Transform.ToLat, "to-lat")) {
                return;
            }
            if (HasInvalidCoordinate(_fields, context.Transform, context.Transform.ToLon, "to-lon")) {
                return;
            }

            _getDistance = r => Get(
                ValueGetter(context.Transform.FromLat)(r),
                ValueGetter(context.Transform.FromLon)(r),
                ValueGetter(context.Transform.ToLat)(r),
                ValueGetter(context.Transform.ToLon)(r)
            );
        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = _getDistance(row);
            Increment();
            return row;
        }

        private Func<IRow, double> ValueGetter(string aliasOrValue) {
            double fromLat;
            if (double.TryParse(aliasOrValue, out fromLat)) {
                return row => fromLat;
            }
            var latFromField = _fields.First(f => f.Alias == aliasOrValue);
            return row => latFromField.Type == "double" ? (double)row[latFromField] : Convert.ToDouble(row[latFromField]);
        }

        public static double Get(double fromLat, double fromLon, double toLat, double toLon) {
            var from = new global::System.Device.Location.GeoCoordinate(fromLat, fromLon);
            var to = new global::System.Device.Location.GeoCoordinate(toLat, toLon);
            return from.GetDistanceTo(to);
        }

        private bool HasInvalidCoordinate(Field[] fields, Configuration.Transform t, string valueOrField, string name) {
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
