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
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class InTransform : BaseTransform {

        private readonly Field _input;
        private readonly HashSet<object> _set = new HashSet<object>();

        public InTransform(IContext context = null) : base(context, "bool") {

            if (IsMissingContext()) {
                return;
            }

            if (IsMissing(Context.Operation.Domain)) {
                return;
            }

            _input = SingleInput();
            var items = Utility.Split(Context.Operation.Domain, ',');
            foreach (var item in items) {
                try {
                    _set.Add(_input.Convert(item));
                } catch (Exception ex) {
                    Context.Warn($"In transform can't convert {item} to {_input.Type} {ex.Message}.");
                }
            }
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _set.Contains(row[_input]);

            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("in") { Parameters = new List<OperationParameter>(1) { new OperationParameter("domain") } };
        }
    }
}