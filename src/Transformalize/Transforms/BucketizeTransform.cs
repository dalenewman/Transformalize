#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2026 Dale Newman
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
using System.Linq;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

   public class BucketizeTransform : BaseTransform {

      private struct BucketEntry {
         public double? Min;  // null = unbounded below ("*")
         public double? Max;  // null = unbounded above ("*")
         public string Label;
      }

      private readonly IField _input;
      private readonly List<BucketEntry> _buckets = new List<BucketEntry>();

      public BucketizeTransform(IContext context = null) : base(context, "string") {

         if (IsMissingContext()) {
            return;
         }

         if (Context.Operation.Map == string.Empty) {
            Error("The bucketize method requires a map");
            Run = false;
            return;
         }

         if (IsNotReceivingNumber()) {
            return;
         }

         var operationMap = Context.Process.Maps.FirstOrDefault(m => m.Name == Context.Operation.Map);
         if (operationMap == null) {
            Error($"The bucketize method cannot find a map named '{Context.Operation.Map}'");
            Run = false;
            return;
         }

         _input = SingleInput();
         Returns = "string";

         foreach (var item in operationMap.Items) {
            var fromStr = item.From?.ToString();
            var toStr = item.To?.ToString();

            double? min = fromStr == "*" ? (double?)null : Convert.ToDouble(item.From);
            double? max = (toStr == "*" || toStr == Constants.DefaultSetting) ? (double?)null : Convert.ToDouble(item.To);

            _buckets.Add(new BucketEntry { Min = min, Max = max, Label = item.Value ?? string.Empty });
         }
      }

      public override IRow Operate(IRow row) {
         var value = Convert.ToDouble(row[_input]);
         foreach (var bucket in _buckets) {
            if ((bucket.Min == null || value >= bucket.Min.Value) &&
                (bucket.Max == null || value <= bucket.Max.Value)) {
               row[Context.Field] = bucket.Label;
               return row;
            }
         }
         row[Context.Field] = row[_input].ToString();
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         return new[] {
            new OperationSignature("bucketize") {
               Parameters = new List<OperationParameter> { new OperationParameter("map") }
            }
         };
      }
   }

}
