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
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

   public class HashcodeTransform : BaseTransform {
      private readonly Field[] _input;
      public HashcodeTransform(IContext context = null) : base(context, "int") {
         if (IsMissingContext()) {
            return;
         }
         if (Context.Field.Name == Constants.TflHashCode && Context.Process.ReadOnly) {
            Run = false;
            return;
         }
         _input = MultipleInput();
      }

      public override IRow Operate(IRow row) {
         row[Context.Field] = GetDeterministicHashCode(_input.Select(f => row[f]));
         return row;
      }

      // get same hash code for string input across separate program execution
      // https://andrewlock.net/why-is-string-gethashcode-different-each-time-i-run-my-program-in-net-core/
      public static int GetDeterministicHashCode(IEnumerable<object> values) {
         var str = string.Concat(values);
         unchecked {
            int hash1 = (5381 << 16) + 5381;
            int hash2 = hash1;

            for (int i = 0; i < str.Length; i += 2) {
               hash1 = ((hash1 << 5) + hash1) ^ str[i];
               if (i == str.Length - 1)
                  break;
               hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
            }

            return hash1 + (hash2 * 1566083941);
         }
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         return new[] { new OperationSignature("hashcode") };
      }
   }
}