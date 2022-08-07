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
using Transformalize.Transforms;

namespace Transformalize.Validators {
    public class NotEqualValidator : BaseValidate {

        private readonly object _value;
        private readonly Func<IRow, bool> _validator;
        private readonly BetterFormat _betterFormat;

        public NotEqualValidator(IContext context = null) : base(context) {
            if (IsMissingContext()) {
                return;
            }

            if (!Run)
                return;

            Field[] rest;
            bool sameTypes;
            var input = MultipleInput();
            var first = input.First();

            if (Context.Operation.Value == Constants.DefaultSetting) {
                rest = input.Skip(1).ToArray();
                sameTypes = rest.All(f => f.Type == first.Type);
            } else {
                _value = first.Convert(Context.Operation.Value);
                rest = input.ToArray();
                sameTypes = input.All(f => f.Type == first.Type);
            }

            if (sameTypes) {
                if (_value == null) {
                    _validator = row => !rest.All(f => row[f].Equals(row[first]));
                } else {
                    _validator = row => !rest.All(f => row[f].Equals(_value));
                }
            } else {
                _validator = row => true;
            }

            var help = Context.Field.Help;
            if (help == string.Empty) {
                if (_value == null) {
                    help = $"{Context.Field.Label} must not equal {{{first.Alias}}} in {first.Label}.";
                } else {
                    help = $"{Context.Field.Label} must not equal {_value}.";
                }
            }
            _betterFormat = new BetterFormat(context, help, Context.Entity.GetAllFields);
        }

        public override IRow Operate(IRow row) {
            if (IsInvalid(row, _validator(row))) {
                AppendMessage(row, _betterFormat.Format(row));
            }

            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("notequal") { Parameters = new List<OperationParameter>(1) { new OperationParameter("value") } };
            yield return new OperationSignature("notequals") { Parameters = new List<OperationParameter>(1) { new OperationParameter("value") } };
            yield return new OperationSignature("unequal") { Parameters = new List<OperationParameter>(1) { new OperationParameter("value") } };
        }
    }
}