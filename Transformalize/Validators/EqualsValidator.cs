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
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Validators {
    public class EqualsValidator : BaseValidate {
        private readonly object _value;
        private readonly Func<IRow, bool> _validator;
        private readonly BetterFormat _betterFormat;

        public EqualsValidator(IContext context) : base(context) {
            if (!Run)
                return;

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
                    _validator = row => rest.All(f => row[f].Equals(row[first]));
                } else {
                    _validator = row => rest.All(f => row[f].Equals(_value));
                }
            } else {
                _validator = row => false;
            }

            var help = context.Field.Help;
            if (help == string.Empty) {
                if (_value == null) {
                    help = $"{context.Field.Label} must equal {{{first.Alias}}} in {first.Label}.";
                } else {
                    help = $"{context.Field.Label} must equal {_value}.";
                }
            }
            _betterFormat = new BetterFormat(context, help, context.Entity.GetAllFields);
        }

        public override IRow Operate(IRow row) {
            var valid = _validator(row);
            row[ValidField] = valid;
            if (!valid) {
                AppendMessage(row, $"Must equal {_value}");
            }
            Increment();
            return row;
        }
    }
}