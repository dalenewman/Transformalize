using System;
using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {


    public class RelativeTimeOperation : ShouldRunOperation {
        private readonly bool _past;
        private readonly long _nowTicks;
        const int SECOND = 1;
        const int MINUTE = 60 * SECOND;
        const int HOUR = 60 * MINUTE;
        const int DAY = 24 * HOUR;
        const int MONTH = 30 * DAY;

        public RelativeTimeOperation(string inKey, string outKey, string fromTimeZone, bool past)
            : base(inKey, outKey) {
            _past = past;
            _nowTicks = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, fromTimeZone).Ticks;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = GetRelativeTime(_nowTicks, ((DateTime)row[InKey]).Ticks, _past);
                } else {
                    Skip();
                }
                yield return row;
            }
        }

        /* http://stackoverflow.com/questions/11/how-do-i-calculate-relative-time */
        public static string GetRelativeTime(long nowTicks, long thenTicks, bool past = true) {

            var suffix = past ? " ago" : string.Empty;
            var ts = past ? new TimeSpan(nowTicks - thenTicks) : new TimeSpan(thenTicks-nowTicks);
            var delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE) {
                return ts.Seconds == 1 ? "one second" + suffix : ts.Seconds + " seconds" + suffix;
            }
            if (delta < 2 * MINUTE) {
                return "a minute" + suffix;
            }
            if (delta < 45 * MINUTE) {
                return ts.Minutes + " minutes" + suffix;
            }
            if (delta < 90 * MINUTE) {
                return "an hour" + suffix;
            }
            if (delta < 24 * HOUR) {
                return ts.Hours + " hours" + suffix;
            }
            if (delta < 48 * HOUR) {
                return past ? "yesterday" : "tomorrow";
            }
            if (delta < 30 * DAY) {
                return ts.Days + " days" + suffix;
            }
            if (delta < 12 * MONTH) {
                var months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month" + suffix : months + " months" + suffix;
            }

            var years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "one year" + suffix : years + " years" + suffix;
        }
    }

}