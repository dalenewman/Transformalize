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
   public class ToUnixTimeTransform : BaseTransform {
      private readonly Field _input;
      private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
      private readonly bool _seconds;

      public ToUnixTimeTransform(IContext context = null) : base(context, "long") {
         if (IsMissingContext()) {
            return;
         }

         if (IsNotReceiving("date")) {
            return;
         }

         _input = SingleInput();

         var allowed = new HashSet<string>() { "ms", "millisecond", "milliseconds", "s", "second", "seconds" };

         if (!allowed.Contains(Context.Operation.TimeComponent)) {
            Run = false;
            Context.Error("The ToUnixTime transform's time component must be set to seconds or milliseconds");
         }

         _seconds = Context.Operation.TimeComponent == "s" || Context.Operation.TimeComponent == "second" || Context.Operation.TimeComponent == "seconds";

      }

      public override IRow Operate(IRow row) {
         var date = (DateTime)row[_input];
         TimeSpan diff = date - epoch;
         row[Context.Field] = Convert.ToInt64(_seconds ? Math.Floor(diff.TotalSeconds) : Math.Floor(diff.TotalMilliseconds));
         return row;
      }

      public override IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {

         if (!Run) {
            foreach (var row in rows) {
               yield return row;
            }
         }

         bool kindTested = false;

         foreach (var row in rows) {
            if (!kindTested) {
               var date = (DateTime)row[_input];
               if (date.Kind != DateTimeKind.Utc) {
                  Context.Warn("The date going into the ToUnixTime transform should be set to UTC by a TimeZone or SpecifyKind transform.");
               }
               kindTested = true;
            }
            yield return Operate(row);
         }
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         return new[] {
            new OperationSignature("tounixtime") {
               Parameters = new List<OperationParameter>(1) {
                  new OperationParameter("time-component")
               }
            }
         };
      }
   }
}