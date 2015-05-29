using System;
using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {
    public class TimeAgoOperation : ShouldRunOperation {
        private readonly long _nowTicks;
        const int SECOND = 1;
        const int MINUTE = 60 * SECOND;
        const int HOUR = 60 * MINUTE;
        const int DAY = 24 * HOUR;
        const int MONTH = 30 * DAY;

        public TimeAgoOperation(string inKey, string outKey, string fromTimeZone)
            : base(inKey, outKey) {
            _nowTicks = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, fromTimeZone).Ticks;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = TimeAgo(_nowTicks, ((DateTime)row[InKey]).Ticks);
                } else {
                    Skip();
                }
                yield return row;
            }
        }

        // http://stackoverflow.com/questions/11/how-do-i-calculate-relative-time
        private static string TimeAgo(long nowTicks, long thenTicks) {

            var ts = new TimeSpan(nowTicks - thenTicks);
            var delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE) {
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
            }
            if (delta < 2 * MINUTE) {
                return "a minute ago";
            }
            if (delta < 45 * MINUTE) {
                return ts.Minutes + " minutes ago";
            }
            if (delta < 90 * MINUTE) {
                return "an hour ago";
            }
            if (delta < 24 * HOUR) {
                return ts.Hours + " hours ago";
            }
            if (delta < 48 * HOUR) {
                return "yesterday";
            }
            if (delta < 30 * DAY) {
                return ts.Days + " days ago";
            }
            if (delta < 12 * MONTH) {
                var months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }

            var years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "one year ago" : years + " years ago";
        }
    }
}