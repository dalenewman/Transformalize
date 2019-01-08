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
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class SortTransform : BaseTransform {
        private readonly Field _input;
        private readonly Func<object, string[]> _sort;
        public SortTransform(IContext context = null) : base(context, "object") {
            if (IsMissingContext()) {
                return;
            }

            if (LastMethodIsNot("split")) {
                return;
            }

            _input = SingleInput();
            if (Context.Operation.Direction == ("asc")) {
                _sort = o => ((string[])o).OrderBy(s => s).ToArray();
            } else {
                _sort = o => ((string[])o).OrderByDescending(s => s).ToArray();
            }
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _sort(row[_input]);
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("sort") { Parameters = new List<OperationParameter>(1) { new OperationParameter("direction", "asc") } };
        }
    }
}