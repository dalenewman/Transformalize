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
using Transformalize.Transforms;

namespace Transformalize.Validators {

    public class IsValidator : StringValidate {

        private readonly Field _input;
        private readonly Func<string, bool> _canConvert;
        private readonly bool _isCompatible;
        private readonly Dictionary<string, Func<string, bool>> _converter = Constants.CanConvert();
        private readonly BetterFormat _betterFormat;

        public IsValidator(IContext context = null) : base(context) {
            if (IsMissingContext()) {
                return;
            }

            if (!Run)
                return;

            if (IsMissing(Context.Operation.Type)) {
                return;
            }
            _input = SingleInput();
            _isCompatible = _input.Type == Context.Operation.Type || _input.IsNumericType() && Context.Operation.Type == "double";
            _canConvert = v => _converter[Context.Operation.Type](v);

            var help = Context.Field.Help;
            if (help == string.Empty) {
                help = $"{_input.Label}'s value is incompatable with the {Context.Operation.Type} data type.";
            }
            _betterFormat = new BetterFormat(context, help, Context.Entity.GetAllFields);
        }

        public override IRow Operate(IRow row) {
            if (IsInvalid(row, _isCompatible || _canConvert(GetString(row, _input)))) {
                AppendMessage(row, _betterFormat.Format(row));
            }

            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("is") { Parameters = new List<OperationParameter>(1) { new OperationParameter("type") } };
        }
    }
}