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

namespace Transformalize.Validators {

    public class DefaultValidator : BaseValidate {
        private readonly Field _input;
        private readonly object _default;

        public DefaultValidator(IContext context) : base(context) {
            if (!Run)
                return;

            _input = SingleInput();
            var types = Constants.TypeDefaults();
            _default = _input.Default == Constants.DefaultSetting ? types[_input.Type] : _input.Convert(_input.Default);
        }

        public override IRow Operate(IRow row) {
            var valid = row[_input].Equals(_default);
            row[ValidField] = valid;
            if (!valid) {
                AppendMessage(row, $"Must be default value of {_default}.");
            }
            Increment();
            return row;
        }
    }
}