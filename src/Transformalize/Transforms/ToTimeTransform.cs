#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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

namespace Transformalize.Transforms {
   public class ToTimeTransform : BaseTransform {

      private readonly Field _input;
      private readonly Func<double, string> _transform;

      public ToTimeTransform(IContext context = null) : base(context, "string") {

         if (IsMissingContext()) {
            return;
         }

         if (!Constants.TimeSpanComponents.Contains(Context.Operation.TimeComponent)) {
            Error($"The {Context.Operation.Method} expects a time component of {Utility.ReadableDomain(Constants.TimeSpanComponents)}.");
            Run = false;
            return;
         }

         if (IsNotReceivingNumber()) {
            return;
         }

         _input = SingleInput();

         switch (Context.Operation.TimeComponent) {
            case "minute":
            case "minutes":
               _transform = (value) => TimeSpan.FromMinutes(value).ToString();
               break;
            case "second":
            case "seconds":
               _transform = (value) => TimeSpan.FromSeconds(value).ToString();
               break;
            case "millisecond":
            case "milliseconds":
               _transform = (value) => TimeSpan.FromMilliseconds(value).ToString();
               break;
            case "tick":
            case "ticks":
               _transform = (value) => TimeSpan.FromTicks(Convert.ToInt64(value)).ToString();
               break;
            case "day":
            case "days":
               _transform = (value) => TimeSpan.FromDays(value).ToString();
               break;
            default:
               _transform = (value) => TimeSpan.FromHours(value).ToString();
               break;
         }


      }

      public override IRow Operate(IRow row) {
         var value = _input.Type == "double" ? (double)row[_input] : Convert.ToDouble(row[_input]);
         row[Context.Field] = _transform(value);
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("totime") {
            Parameters = new List<OperationParameter> {
               new OperationParameter("time-component")
            }
         };
      }

   }
}