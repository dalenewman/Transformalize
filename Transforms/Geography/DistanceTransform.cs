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
using System.Globalization;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.Geography {
    public class DistanceTransform : BaseTransform {

        private readonly Field[] _fields;
        private readonly Func<IRow, double> _getDistance;
        private int _emptyStringCount;
        private int _nonNumericCount;
        private readonly HashSet<string> _nonNumericValues = new HashSet<string>();

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
                if (Math.Abs(fromLat) > 90) {
                    Context.Warn($"You are passing an invalid from latitude of {fromLat} into a distance transform in {Context.Field.Alias}");
                    return -1;
                }
                if (Math.Abs(toLat) > 90) {
                    Context.Warn($"You are passing an invalid to latitude of {toLat} into a distance transform in {Context.Field.Alias}");
                    return -1;
                }
                if (Math.Abs(fromLon) > 180) {
                    Context.Warn($"You are passing an invalid from longitude of {fromLon} into a distance transform in {Context.Field.Alias}");
                    return -1;
                }
                if (Math.Abs(toLon) > 180) {
                    Context.Warn($"You are passing an invalid to longitude of {toLon} into a distance transform in {Context.Field.Alias}");
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
                        _emptyStringCount++;
                        return default(double);
                    } else {
                        if (double.TryParse(stringValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var retNum)) {
                            return retNum;
                        }
                        _nonNumericCount++;
                        if (_nonNumericValues.Count < 5) {
                            _nonNumericValues.Add(stringValue);
                        }
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

        public override void Dispose() {
            if (_emptyStringCount > 0) {
                Context.Warn($"You are passing {_emptyStringCount} empty strings into a distance transform in {Context.Field.Alias}");
            }
            if (_nonNumericCount > 0) {
                Context.Warn($"You are passing {_nonNumericCount} non numeric values into a distance transform in {Context.Field.Alias}");
                foreach (var value in _nonNumericValues) {
                    Context.Warn($"Non numeric values include: {value}");
                }
            }
            base.Dispose();

        }
    }
}
