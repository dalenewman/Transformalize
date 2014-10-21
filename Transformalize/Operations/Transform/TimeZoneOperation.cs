using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Logging;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {

    public class TimeZoneOperation : ShouldRunOperation {

        private readonly TimeZoneInfo _toTimeZoneInfo;
        private readonly TimeSpan _adjustment;
        private readonly TimeSpan _daylightAdjustment;

        public TimeZoneOperation(string inKey, string outKey, string fromTimeZone, string toTimeZone)
            : base(inKey, outKey) {

            var fromTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(fromTimeZone);
            _toTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(toTimeZone);

            _adjustment = _toTimeZoneInfo.BaseUtcOffset - fromTimeZoneInfo.BaseUtcOffset;
            _daylightAdjustment = _adjustment.Add(new TimeSpan(0, 1, 0, 0));

            Name = string.Format("TimeZoneOperation ({0})", outKey);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var date = (DateTime)row[InKey];
                    if (_toTimeZoneInfo.IsDaylightSavingTime(DateTime.Now)) {
                        row[OutKey] = date.Add(_daylightAdjustment);
                    } else {
                        row[OutKey] = date.Add(_adjustment);
                    }
                    //if (_utcToLocal) {
                    //    row[OutKey] = new DateTime(date.Ticks, DateTimeKind.Local);
                    //}
                } else {
                    Skip();
                }

                yield return row;
            }
        }

        public static string GuardTimeZone(string process, string entity, string timeZone, string defaultTimeZone) {
            var result = timeZone;
            if (timeZone == String.Empty) {
                result = defaultTimeZone;
                TflLogger.Debug(process, entity, "Defaulting From TimeZone to {0}.", defaultTimeZone);
            } else {
                if (!TimeZoneInfo.GetSystemTimeZones().Any(tz => tz.Id.Equals(timeZone))) {
                    throw new TransformalizeException("From Timezone Id {0} is invalid.", timeZone);
                }
            }
            return result;
        }

    }
}