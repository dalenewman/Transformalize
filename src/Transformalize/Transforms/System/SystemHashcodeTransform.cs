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
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.System {
   public class SystemHashcodeTransform : BaseTransform {

      private readonly Field _tflHashCode;
      private readonly Field[] _hashFields;

      public SystemHashcodeTransform(IContext context) : base(context, null) {

         if (Context.Process.ReadOnly) {
            Run = false;
            return;
         }

         _tflHashCode = context.Entity.TflHashCode();
         _hashFields = context.Entity.Fields.Where(f => f.Input && !f.PrimaryKey).OrderBy(f => f.Input).ToArray();

      }

      public override IRow Operate(IRow row) {
         row[_tflHashCode] = HashcodeTransform.GetDeterministicHashCode(_hashFields.Select(f => row[f]));
         return row;
      }

   }
}