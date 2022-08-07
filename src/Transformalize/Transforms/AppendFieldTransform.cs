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
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class AppendFieldTransform : StringTransform {

        private readonly Field _input;
        private readonly IField _field;

        public AppendFieldTransform(IContext context, IField field) : base(context, "string") {

            _field = field;

            _input = SingleInput();

            if (Received() != "string") {
                Warn($"Appending to a {Received()} type in {Context.Field.Alias} converts it to a string.");
            }
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = row[_input] + GetString(row, _field);
            return row;
        }

    }
}