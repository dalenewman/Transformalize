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
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Validators {
    public class NumericValidator : StringValidate {
        private readonly Func<IRow, bool> _validator;
        private readonly BetterFormat _betterFormat;

        public NumericValidator(IContext context = null) : base(context) {
            if (IsMissingContext()) {
                return;
            }

            if (!Run)
                return;

            var input = SingleInput();
            if (input.IsNumeric()) {
                _validator = row => true;
            } else {
                _validator = row => double.TryParse(GetString(row, input), out _);
            }
            var help = Context.Field.Help;
            if (help == string.Empty) {
                help = $"The value {{{Context.Field.Alias}}} in {Context.Field.Label} must be numeric.";
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
            yield return new OperationSignature("numeric");
        }
    }
}