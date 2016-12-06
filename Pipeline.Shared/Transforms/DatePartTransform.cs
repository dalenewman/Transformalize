#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class DatePartTransform : BaseTransform {

        public static readonly Dictionary<string, Func<DateTime, object>> Parts = new Dictionary<string, Func<DateTime, object>>() {
            {"day", x => x.Day},
            {"date", x=>x.Date},
            {"dayofweek", x=>x.DayOfWeek.ToString()},
            {"dayofyear", x=>x.DayOfYear},
            {"hour", x=>x.Hour},
            {"millisecond", x=>x.Millisecond},
            {"minute",x=>x.Minute},
            {"month",x=>x.Month},
            {"second",x=>x.Second},
            {"tick",x=>x.Ticks},
            {"weekofyear",x=> CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(x,CalendarWeekRule.FirstDay, DayOfWeek.Sunday)},
            {"year",x=>x.Year}
        };

        public static readonly Dictionary<string, string> PartReturns = new Dictionary<string, string>() {
            {"day", "int"},
            {"date", "datetime"},
            {"dayofweek", "string"},
            {"dayofyear", "int"},
            {"hour", "int"},
            {"millisecond", "int"},
            {"minute","int"},
            {"month","int"},
            {"second","int"},
            {"tick","long"},
            {"year","int"},
            {"weekofyear","int" }
        };

        private readonly Field _input;
        private readonly Func<IRow, object> _transform;

        public DatePartTransform(IContext context) : base(context, PartReturns[context.Transform.TimeComponent]) {
            _input = SingleInput();
            _transform = row => Parts[context.Transform.TimeComponent]((DateTime)row[_input]);
        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = _transform(row);
            Increment();
            return row;
        }
    }
}