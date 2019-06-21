#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
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
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.Dates {

   public class TimeZoneTransform : BaseTransform {

      private readonly Field _input;
      private readonly Func<DateTime, object> _transform;

      public TimeZoneTransform(IContext context = null) : base(context, "datetime") {
         if (IsMissingContext()) {
            return;
         }

         if (IsNotReceiving("date")) {
            return;
         }

         if (IsMissing(Context.Operation.FromTimeZone)) {
            return;
         }

         if (IsMissing(Context.Operation.ToTimeZone)) {
            return;
         }

         _input = SingleInput();

#if NETS10
         Run = false;
         Context.Error($"The timezone transform in {Context.Field.Alias} is not yet implemented on .net standard 1.0.");
         _transform = null;
#else

         var fromTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(Context.Operation.FromTimeZone);
         var toTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(Context.Operation.ToTimeZone);

         if (fromTimeZoneInfo.StandardName == "UTC") {
            _transform = (dt) => {
               if (dt.Kind != DateTimeKind.Utc) {
                  dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
               }
               return TimeZoneInfo.ConvertTimeFromUtc(dt, toTimeZoneInfo);
            };
         } else if (toTimeZoneInfo.StandardName == "UTC") {
            _transform = (dt) => TimeZoneInfo.ConvertTimeToUtc(dt, fromTimeZoneInfo);
         } else {
            _transform = (dt) => {
               var utc = TimeZoneInfo.ConvertTimeToUtc(dt, fromTimeZoneInfo);
               return TimeZoneInfo.ConvertTimeFromUtc(utc, toTimeZoneInfo);
            };
         }
#endif
      }

      public override IRow Operate(IRow row) {
         var date = (DateTime)row[_input];
         row[Context.Field] = _transform(date);
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         return new[]{ new OperationSignature("timezone") {
                Parameters = new List<OperationParameter>(2) {
                    new OperationParameter("from-time-zone"),
                    new OperationParameter("to-time-zone")
                }
            }};
      }
   }
}