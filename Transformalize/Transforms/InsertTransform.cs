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
    public class InsertTransform : BaseTransform {
        private readonly Field _input;

        public InsertTransform(IContext context) : base(context, "string") {
            if (IsNotReceiving("string")) {
                return;
            }

            if (context.Transform.StartIndex == 0) {
                Error("The insert transform requires a start-index greater than 0.");
                Run = false;
                return;
            }
            if (context.Transform.Value == string.Empty) {
                Warn("The insert transform should have a value to insert.");
            }

            _input = SingleInput();
        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = row[_input].ToString().Insert(Context.Transform.StartIndex, Context.Transform.Value);
            Increment();
            return row;
        }
    }
}