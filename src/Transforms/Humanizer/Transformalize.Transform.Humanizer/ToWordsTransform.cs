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
using System;
using System.Collections.Generic;
using Humanizer;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Transforms.Humanizer {
    public class ToWordsTransform : BaseTransform {
        private readonly Func<IRow, object> _transform;

        public ToWordsTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext()) {
                return;
            }
            if (IsNotReceivingNumber()) {
                return;
            }

            var input = SingleInput();
            var left3 = input.Type.Left(3);
            switch (left3) {
                case "int":
                case "sho":
                    _transform = (row) => {
                        var value = left3 == "int" ? (int)row[input] : Convert.ToInt32(row[input]);
                        return value.ToWords();
                    };
                    break;
                default:
                    _transform = (row) => row[input];
                    break;
            }
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _transform(row);
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            return new[] { new OperationSignature("towords") };
        }
    }
}