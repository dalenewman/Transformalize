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
    public class TrimStartTransform : StringTransform {

        private readonly Field _input;
        private readonly char[] _trimChars;

        public TrimStartTransform(IContext context) : base(context, "string") {
            if (IsMissing(context.Operation.TrimChars)) {
                return;
            }

            _input = SingleInput();
            context.Operation.TrimChars = context.Operation.TrimChars.Replace("\\r", "\r");
            context.Operation.TrimChars = context.Operation.TrimChars.Replace("\\n", "\n");
            _trimChars = Context.Operation.TrimChars.ToCharArray();
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = GetString(row, _input).TrimStart(_trimChars);
            
            return row;
        }

    }
}