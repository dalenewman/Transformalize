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

        public IsValidator(IContext context) : base(context) {
            if (!Run)
                return;

            if (IsMissing(context.Operation.Type)) {
                return;
            }
            _input = SingleInput();
            _isCompatible = _input.Type == context.Operation.Type || _input.IsNumeric() && context.Operation.Type == "double";
            _canConvert = v => _converter[context.Operation.Type](v);

            var help = context.Field.Help;
            if (help == string.Empty) {
                help = $"{_input.Label}'s value is incompatable with the {context.Operation.Type} data type.";
            }
            _betterFormat = new BetterFormat(context, help, context.Entity.GetAllFields);
        }

        public override IRow Operate(IRow row) {
            var valid = _isCompatible || _canConvert(GetString(row, _input));
            row[ValidField] = valid;
            if (!valid) {
                AppendMessage(row, _betterFormat.Format(row));
            }
            
            return row;
        }

    }
}