using System;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms {

    public class DateAddTransform : BaseTransform {

        private readonly Field _input;
        private readonly TimeSpan _amount;

        public DateAddTransform(IContext context, string unit) : base(context, "datetime") {

            _input = SingleInput();
            var value = Convert.ToDouble(context.Transform.Value);

            switch (unit.ToLower()) {
                case "second":
                case "seconds":
                    _amount = TimeSpan.FromSeconds(value);
                    break;
                case "millisecond":
                case "milliseconds":
                    _amount = TimeSpan.FromMilliseconds(value);
                    break;
                case "day":
                case "days":
                    _amount = TimeSpan.FromDays(value);
                    break;
                case "hour":
                case "hours":
                    _amount = TimeSpan.FromHours(value);
                    break;
                case "minute":
                case "minutes":
                    _amount = TimeSpan.FromMinutes(value);
                    break;
                case "tick":
                case "ticks":
                    _amount = TimeSpan.FromTicks(Convert.ToInt64(context.Transform.Value));
                    break;
                default:
                    context.Warn($"Add time does not support {unit}. No time being added");
                    _amount = new TimeSpan();
                    break;

            }
        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = ((DateTime)row[_input]).Add(_amount);
            Increment();
            return row;
        }
    }
}