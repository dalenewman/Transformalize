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
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class FormatTransform : BaseTransform {
        private readonly Field[] _input;

        public FormatTransform(IContext context)
            : base(context, "string") {
            if (IsNotReceiving("string")) {
                return;
            }

            if (context.Operation.Format == string.Empty) {
                Error($"The format transform in field {context.Field.Alias} requires a format parameter.");
                Run = false;
                return;
            }

            if (context.Operation.Format.IndexOf('{') == -1) {
                Error("The format transform's format must contain a curly braced place-holder.");
                Run = false;
                return;
            }

            if (context.Operation.Format.IndexOf('}') == -1) {
                Error("The format transform's format must contain a curly braced place-holder.");
                Run = false;
                return;
            }

            _input = MultipleInput();
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = string.Format(Context.Operation.Format, _input.Select(f => row[f]).ToArray());
            Increment();
            return row;
        }

    }
}