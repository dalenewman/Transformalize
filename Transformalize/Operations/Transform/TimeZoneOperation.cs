using System;
using System.Collections.Generic;
using Transformalize.Libs.NLog.Internal;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations.Transform {

    public class TimeZoneOperation : AbstractOperation {

        private readonly string _inKey;
        private readonly string _outKey;
        private readonly TimeZoneInfo _toTimeZoneInfo;
        private readonly TimeSpan _adjustment;
        private readonly TimeSpan _daylightAdjustment;

        public TimeZoneOperation(string inKey, string outKey, string fromTimeZone, string toTimeZone) {
            _inKey = inKey;
            _outKey = outKey;

            fromTimeZone = GuardTimeZone(fromTimeZone, "UTC");
            toTimeZone = GuardTimeZone(toTimeZone, TimeZoneInfo.Local.Id);

            var fromTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(fromTimeZone);
            _toTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(toTimeZone);

            _adjustment = _toTimeZoneInfo.BaseUtcOffset - fromTimeZoneInfo.BaseUtcOffset;
            _daylightAdjustment = _adjustment.Add(new TimeSpan(0, 1, 0, 0));

        }

        private string GuardTimeZone(string timeZone, string defaultTimeZone) {
            var result = timeZone;
            if (timeZone == string.Empty) {
                result = defaultTimeZone;
                Debug("Defaulting From TimeZone to {0}.", defaultTimeZone);
            } else {
                if (!TimeZoneInfo.GetSystemTimeZones().Any(tz => tz.Id.Equals(timeZone))) {
                    Error("From Timezone Id {0} is invalid.", timeZone);
                    Environment.Exit(1);
                }
            }
            return result;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                var date = (DateTime)row[_inKey];
                if (_toTimeZoneInfo.IsDaylightSavingTime(DateTime.Now)) {
                    row[_outKey] = date.Add(_daylightAdjustment);
                } else {
                    row[_outKey] = date.Add(_adjustment);
                }
                yield return row;
            }
        }
    }
}