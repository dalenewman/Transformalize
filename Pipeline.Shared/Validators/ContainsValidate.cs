#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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

using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Validators {
    public class ContainsValidator : BaseTransform {

        readonly Field[] _input;

        public ContainsValidator(IContext context) : base(context, "bool") {

            _input = MultipleInput();

        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = _input.Any(f => row[f].ToString().Contains(Context.Transform.Value));
            Increment();
            return row;
        }

    }
}