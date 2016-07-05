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
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    public class ToTimeTransform : BaseTransform, ITransform {
        private readonly Field _input;
        public ToTimeTransform(IContext context) : base(context) {
            _input = SingleInput();


        }

        // "day,date,dayofweek,dayofyear,hour,millisecond,minute,month,second,tick,year,weekofyear", toLower = true)]
        public IRow Transform(IRow row) {
            var value = _input.Type == "double" ? (double)row[_input] : Convert.ToDouble(row[_input]);
            switch (Context.Transform.TimeComponent) {
                case "minute":
                    row[Context.Field] = TimeSpan.FromMinutes(value).ToString();
                    break;
                case "second":
                    row[Context.Field] = TimeSpan.FromSeconds(value).ToString();
                    break;
                case "millisecond":
                    row[Context.Field] = TimeSpan.FromMilliseconds(value).ToString();
                    break;
                case "tick":
                    row[Context.Field] = TimeSpan.FromTicks(Convert.ToInt64(row[_input])).ToString();
                    break;
                case "day":
                    row[Context.Field] = TimeSpan.FromDays(value).ToString();
                    break;
                default:
                    row[Context.Field] = TimeSpan.FromHours(value).ToString();
                    break;
            }
            Increment();
            return row;
        }
    }
}