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
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class EqualsTransform : BaseTransform {

        private readonly object _value;
        private readonly Action<IRow> _validator;

        public EqualsTransform(IContext context = null) : base(context, "bool") {

            if (IsMissingContext()) {
                return;
            }

            Field[] rest;
            bool sameTypes;
            var input = MultipleInput();
            var first = input.First();

            if (context.Operation.Value == Constants.DefaultSetting) {
                rest = input.Skip(1).ToArray();
                sameTypes = rest.All(f => f.Type == first.Type);
            } else {
                _value = first.Convert(context.Operation.Value);
                rest = input.ToArray();
                sameTypes = input.All(f => f.Type == first.Type);
            }

            if (sameTypes) {
                if (_value == null) {
                    _validator = row => row[Context.Field] = rest.All(f => row[f].Equals(row[first]));
                } else {
                    _validator = row => row[Context.Field] = rest.All(f => row[f].Equals(_value));
                }
            } else {
                _validator = row => row[Context.Field] = false;
            }
        }

        public override IRow Operate(IRow row) {
            _validator(row);
            
            return row;
        }

        public new IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature {
                Method = "all",
                Parameters = new List<OperationParameter>{
                    new OperationParameter("value", "[default]")
                }
            };
            yield return new OperationSignature {
                Method = "equals",
                Parameters = new List<OperationParameter>{
                    new OperationParameter("value", "[default]")
                }
            };
        }
    }
}