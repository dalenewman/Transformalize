using System;
using System.Collections.Generic;
using System.Globalization;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Transform {
    public class WeekOfYearOperation : ShouldRunOperation {

        private readonly CalendarWeekRule _calendarWeekRule;
        private readonly DayOfWeek _dayOfWeek;

        public WeekOfYearOperation(string inKey, string outKey, CalendarWeekRule calendarWeekRule, DayOfWeek dayOfWeek)
            : base(inKey, outKey) {
            _calendarWeekRule = calendarWeekRule;
            _dayOfWeek = dayOfWeek;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear((DateTime)row[InKey], _calendarWeekRule, _dayOfWeek);
                } else { Skip(); }
                yield return row;
            }
        }
    }
}