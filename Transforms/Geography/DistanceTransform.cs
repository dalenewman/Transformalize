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
using System.Text.RegularExpressions;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Transforms.Geography {
    public class DistanceTransform : BaseTransform {
        private readonly Field[] _fields;
        private readonly Func<IRow, double> _getDistance;

        public DistanceTransform(IContext context) : base(context, "double") {

            _fields = context.GetAllEntityFields().ToArray();

            if (HasInvalidCoordinate(_fields, context.Operation, context.Operation.FromLat, "from-lat")) {
                return;
            }
            if (HasInvalidCoordinate(_fields, context.Operation, context.Operation.FromLon, "from-lon")) {
                return;
            }
            if (HasInvalidCoordinate(_fields, context.Operation, context.Operation.ToLat, "to-lat")) {
                return;
            }
            if (HasInvalidCoordinate(_fields, context.Operation, context.Operation.ToLon, "to-lon")) {
                return;
            }

            _getDistance = r => {
                var fromLat = ValueGetter(context.Operation.FromLat)(r);
                var fromLon = ValueGetter(context.Operation.FromLon)(r);
                var toLat = ValueGetter(context.Operation.ToLat)(r);
                var toLon = ValueGetter(context.Operation.ToLon)(r);
                if (Math.Abs(fromLat) > 90 || Math.Abs(toLat) > 90 || Math.Abs(fromLon) > 180 || Math.Abs(toLon) > 180) {
                    Context.Warn($"You are passing an invalid latitude or longitude into distance transform in {Context.Field}");
                    return -1;
                }
                return Get(fromLat, fromLon, toLat, toLon);
            };
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _getDistance(row);
            Increment();
            return row;
        }

        private Func<IRow, double> ValueGetter(string aliasOrValue) {
            if (double.TryParse(aliasOrValue, out var fromLat)) {
                return row => fromLat;
            }
            var field = _fields.First(f => f.Alias == aliasOrValue);

            return row => field.Type == "double" ? (double)row[field] : ConvertValue(field, row[field]);
        }

        private double ConvertValue(IField field, object value) {
            switch (field.Type) {
                case "string":
                    var stringValue = value as string;
                    if (string.IsNullOrEmpty(stringValue)) {
                        Context.Warn($"You are passing empty string into distance transform in {Context.Field}");
                        return default(double);
                    } else {
                        if (stringValue.IsNumeric()) {
                            return Convert.ToDouble(value);
                        }
                        Context.Warn($"You are passing a non numeric value of {stringValue} into distance transform in {Context.Field}");
                        return default(double);
                    }
                default:
                    return Convert.ToDouble(value);
            }

        }

        public static double Get(double fromLat, double fromLon, double toLat, double toLon) {
            var from = new global::System.Device.Location.GeoCoordinate(fromLat, fromLon);
            var to = new global::System.Device.Location.GeoCoordinate(toLat, toLon);
            return from.GetDistanceTo(to);
        }

        private bool HasInvalidCoordinate(Field[] fields, Operation t, string valueOrField, string name) {

            if (fields.All(f => f.Alias != valueOrField) && !double.TryParse(valueOrField, out var doubleValue)) {
                Error($"The {t.Method} method's {name} parameter: {valueOrField}, is not a valid field or numeric value.");
                Run = false;
                return true;
            }
            return false;
        }


    }
}
