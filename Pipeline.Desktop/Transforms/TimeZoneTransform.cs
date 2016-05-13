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
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Transforms;

namespace Pipeline.Desktop.Transforms {
    public class TimeZoneTransform : BaseTransform, ITransform {
        readonly Field _input;
        readonly Field _output;
        private readonly TimeZoneInfo _toTimeZoneInfo;
        private readonly TimeSpan _adjustment;
        private readonly TimeSpan _daylightAdjustment;

        public TimeZoneTransform(PipelineContext context) : base(context) {
            _input = SingleInput();
            _output = context.Field;

            var fromTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(context.Transform.FromTimeZone);
            _toTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(context.Transform.ToTimeZone);

            _adjustment = _toTimeZoneInfo.BaseUtcOffset - fromTimeZoneInfo.BaseUtcOffset;
            _daylightAdjustment = _adjustment.Add(new TimeSpan(0, 1, 0, 0));
        }

        public IRow Transform(IRow row) {
            Increment();
            var date = (DateTime)row[_input];
            if (_toTimeZoneInfo.IsDaylightSavingTime(date)) {
                row[_output] = date.Add(_daylightAdjustment);
            } else {
                row[_output] = date.Add(_adjustment);
            }
            return row;
        }
    }
}