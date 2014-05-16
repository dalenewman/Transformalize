using System;
using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Operations.Transform;

namespace Transformalize.Main {
    public class DatePartOperation : ShouldRunOperation {

        private readonly string _timeComponent;
        private readonly Dictionary<string, Func<DateTime, object>> _parts = new Dictionary<string, Func<DateTime, object>>() {
            {"day", x => x.Day},
            {"date", x=>x.Date},
            {"dayofweek", x=>x.DayOfWeek},
            {"dayofyear", x=>x.DayOfYear},
            {"hour", x=>x.Hour},
            {"millisecond", x=>x.Millisecond},
            {"minute",x=>x.Minute},
            {"month",x=>x.Month},
            {"second",x=>x.Second},
            {"tick",x=>x.Ticks},
            {"year",x=>x.Year}
        };

        public DatePartOperation(string inKey, string outKey, string outType, string timeComponent)
            : base(inKey, outKey) {

            _timeComponent = timeComponent.ToLower().TrimEnd("s".ToCharArray());
            if (!_parts.ContainsKey(_timeComponent)) {
                throw new TransformalizeException("DatePart does not handle {0} time component. Set time-component to day, date, dayofweek, dayofyear, hour, millisecond, minute, month, second, tick, or year.", _timeComponent);
            }

            var testValue = _parts[_timeComponent](DateTime.Now);
            if (!CanChangeType(testValue, Common.ToSystemType(outType))) {
                throw new TransformalizeException("DatePart can't change type from {0} to {1}.", testValue.GetType(), outType);
            }

            Name = string.Format("DatePart ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    if (row[InKey] is DateTime) {
                        var date = ((DateTime)row[InKey]);
                        row[OutKey] = _parts[_timeComponent](date);
                    }
                }
                yield return row;
            }
        }

        public static bool CanChangeType(object value, Type conversionType) {
            if (conversionType == null) {
                return false;
            }

            if (value == null) {
                return false;
            }

            var convertible = value as IConvertible;

            return convertible != null;
        }
    }
}