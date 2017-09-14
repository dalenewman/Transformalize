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

namespace Transformalize.Validators {
    public class ContainsValidator : StringValidate {
        private readonly Field[] _input;
        private readonly Field _valueField;
        private readonly bool _valueIsField;

        public ContainsValidator(IContext context) : base(context) {

            if (!Run)
                return;

            if (IsMissing(context.Operation.Value)) {
                return;
            }
            _input = MultipleInput();
            _valueIsField = context.Entity.TryGetField(context.Operation.Value, out _valueField);

        }

        public override IRow Operate(IRow row) {
            var valueItMustContain = _valueIsField ? GetString(row, _valueField) : Context.Operation.Value;
            var valid = _input.Any(f => GetString(row, f).Contains(valueItMustContain));
            row[ValidField] = valid;
            if (!valid) {
                AppendMessage(row, $"Must contain {valueItMustContain}.");
            }
            Increment();
            return row;
        }

    }
}