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
    public class ContainsValidator : StringTransform {
        private readonly Field[] _input;
        private readonly Field _valueField;
        private readonly bool _valueIsField;
        private readonly Field _outputField;
        private readonly Field _messageField;
        private readonly bool _hasMessageField;

        public ContainsValidator(IContext context) : base(context, "bool") {
            if (IsMissing(context.Transform.Value)) {
                return;
            }
            _input = MultipleInput();
            _valueIsField = context.Entity.TryGetField(context.Transform.Value, out _valueField);

            if (!context.Entity.TryGetField(context.Field.ValidField, out _outputField)) {
                _outputField = context.Field;
            }

            _hasMessageField = context.Entity.TryGetField(context.Field.ValidMessageField, out _messageField);

        }

        public override IRow Transform(IRow row) {
            var valueItMustContain = _valueIsField ? GetString(row, _valueField) : Context.Transform.Value;
            var valid = _input.Any(f => GetString(row, f).Contains(valueItMustContain));
            if (!valid && _hasMessageField) {
                row[_messageField] = GetString(row, _messageField) + $"Must contain {valueItMustContain}. ";
            }
            row[_outputField] = valid;
            Increment();
            return row;
        }

    }
}