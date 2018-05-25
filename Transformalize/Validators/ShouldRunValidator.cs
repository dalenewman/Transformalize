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
using System.Collections.Generic;
using Transformalize.Contracts;

namespace Transformalize.Validators {
    public class ShouldRunValidator : BaseValidate {

        private readonly IValidate _validator;

        public ShouldRunValidator(IContext context, IValidate validator) : base(context) {
            _validator = validator;
        }

        public override IRow Operate(IRow row) {
            return Context.Operation.ShouldRun(row) ? _validator.Operate(row) : row;
        }

        public override IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {
            return _validator.Operate(rows);
        }

        public new IEnumerable<string> Errors() {
            return _validator.Errors();
        }

        public new IEnumerable<string> Warnings() {
            return _validator.Warnings();
        }
    }
}