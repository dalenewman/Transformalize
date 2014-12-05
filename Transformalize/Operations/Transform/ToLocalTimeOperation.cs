using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {

    public class ToLocalTimeOperation : ShouldRunOperation {

        private readonly TimeSpan _adjustment;

        public ToLocalTimeOperation(string inKey, string outKey, string fromTimeZone, string toTimeZone)
            : base(inKey, outKey) {

            fromTimeZone = GuardTimeZone(fromTimeZone, "UTC");
            toTimeZone = GuardTimeZone(toTimeZone, TimeZoneInfo.Local.Id);

            var fromTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(fromTimeZone);
            var toTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(toTimeZone);

            _adjustment = toTimeZoneInfo.BaseUtcOffset - fromTimeZoneInfo.BaseUtcOffset;

            if (toTimeZoneInfo.IsDaylightSavingTime(DateTime.Now)) {
                _adjustment = _adjustment.Add(new TimeSpan(0, 1, 0, 0));
            }

            Name = string.Format("ToLocalTimeOperation ({0})", outKey);
        }

        private string GuardTimeZone(string timeZone, string defaultTimeZone) {
            var result = timeZone;
            if (timeZone == string.Empty) {
                result = defaultTimeZone;
                Debug("Defaulting From TimeZone to {0}.", defaultTimeZone);
            } else {
                if (!TimeZoneInfo.GetSystemTimeZones().Any(tz => tz.Id.Equals(timeZone))) {
                    throw new TransformalizeException(ProcessName, EntityName, "From Timezone Id {0} is invalid.", timeZone);
                }
            }
            return result;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var date = (DateTime)row[InKey];
                    row[OutKey] = date.Add(_adjustment);
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }
    }
}