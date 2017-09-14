#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class ReplaceTransform : StringTransform {
        private readonly Field _input;

        public ReplaceTransform(IContext context) : base(context, "string") {
            if (IsMissing(context.Operation.OldValue)) {
                return;
            }

            _input = SingleInput();

            context.Operation.OldValue = context.Operation.OldValue.Replace("\\r", "\r");
            context.Operation.OldValue = context.Operation.OldValue.Replace("\\n", "\n");
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = GetString(row,_input).Replace(Context.Operation.OldValue, Context.Operation.NewValue);
            Increment();
            return row;
        }


    }
}