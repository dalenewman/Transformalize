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
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Validators {

    public class StartsWithValidator : StringValidate {

        private readonly Field _input;
        private readonly BetterFormat _betterFormat;

        public StartsWithValidator(IContext context) : base(context) {
            if (!Run) {
                return;
            }

            if (IsMissing(context.Operation.Value)) {
                return;
            }
            _input = SingleInput();
            var help = context.Field.Help;
            if (help == string.Empty) {
                help = $"{context.Field.Label} must start with {context.Operation.Value}.";
            }
            _betterFormat = new BetterFormat(context, help, context.Entity.GetAllFields);
        }

        public override IRow Operate(IRow row) {
            var value = GetString(row, _input);
            var valid = value.StartsWith(Context.Operation.Value);
            row[ValidField] = valid;
            if (!valid) {
                AppendMessage(row, _betterFormat.Format(row));
            }
            
            return row;
        }
    }
}