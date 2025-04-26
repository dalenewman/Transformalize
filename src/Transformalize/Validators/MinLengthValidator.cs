#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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
using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Validators {
    public class MinLengthValidator : StringValidate {
        private readonly Field _input;
        private readonly BetterFormat _betterFormat;

        public MinLengthValidator(IContext context = null) : base(context) {
            if (IsMissingContext()) {
                return;
            }

            if (!Run)
                return;
            _input = SingleInput();
            var help = Context.Field.Help;
            if (help == string.Empty) {
                help = $"{Context.Field.Label} must have {Context.Operation.Length} or more characters.";
            }
            _betterFormat = new BetterFormat(context, help, Context.Entity.GetAllFields);
        }

        public override IRow Operate(IRow row) {
            if (IsInvalid(row, GetString(row, _input).Length >= Context.Operation.Length)) {
                AppendMessage(row, _betterFormat.Format(row));
            }

            return row;
        }


        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("minlength") { Parameters = new List<OperationParameter>(1) { new OperationParameter("length") } };
        }
    }
}