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

namespace Transformalize.Transforms {
    public class AppendTransform : BaseTransform {

        private readonly Field _input;
        private readonly bool _isField;
        private readonly Field _field;

        public AppendTransform(IContext context) : base(context, "string") {
            _input = SingleInput();
            _isField = context.Entity.FieldMatcher.IsMatch(context.Operation.Value);
            if (_isField) {
                _field = context.Entity.GetField(context.Operation.Value);
            }
            Run = context.Operation.Value != Constants.DefaultSetting;

            if (Run) {
                if (Received() != "string") {
                    Warn($"Appending to a {Received()} type in {context.Field.Alias} converts it to a string.");
                }
            } else {
                Warn($"Appending nothing in {context.Field.Alias}.");
            }
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = row[_input] + (_isField ? row[_field].ToString() : Context.Operation.Value);
            return row;
        }

    }

}