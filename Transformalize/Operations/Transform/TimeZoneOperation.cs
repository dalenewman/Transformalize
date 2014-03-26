using System;
using System.Collections.Generic;
using System.Threading;
using Transformalize.Libs.NLog;
using Transformalize.Libs.NLog.Internal;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {

    public class TimeZoneOperation : ShouldRunOperation {

        private readonly TimeZoneInfo _toTimeZoneInfo;
        private readonly TimeSpan _adjustment;
        private readonly TimeSpan _daylightAdjustment;

        public TimeZoneOperation(string inKey, string outKey, string fromTimeZone, string toTimeZone)
            : base(inKey, outKey) {

            fromTimeZone = Common.GuardTimeZone(fromTimeZone, "UTC");
            toTimeZone = Common.GuardTimeZone(toTimeZone, TimeZoneInfo.Local.Id);

            var fromTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(fromTimeZone);
            _toTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(toTimeZone);

            _adjustment = _toTimeZoneInfo.BaseUtcOffset - fromTimeZoneInfo.BaseUtcOffset;
            _daylightAdjustment = _adjustment.Add(new TimeSpan(0, 1, 0, 0));

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
                } else {
                    Interlocked.Increment(ref SkipCount);
                }

                yield return row;
            }
        }
    }
}