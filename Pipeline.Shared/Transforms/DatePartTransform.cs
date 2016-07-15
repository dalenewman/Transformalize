#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    public class DatePartTransform : BaseTransform, ITransform {

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
        private readonly Action<IRow> _transform;

        public DatePartTransform(IContext context) : base(context) {
            _input = SingleInput();
            if (_input.Type == PartReturns[context.Transform.TimeComponent]) {
                _transform = row => row[context.Field] = Parts[context.Transform.TimeComponent]((DateTime)row[_input]);
            } else {
                _transform = row => row[context.Field] = context.Field.Convert(Parts[context.Transform.TimeComponent]((DateTime)row[_input]));
            }
        }

        public IRow Transform(IRow row) {
            _transform(row);
            Increment();
            return row;
        }
    }
}