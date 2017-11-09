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

namespace Transformalize.Transforms.System {
    public class ShouldRunTransform : BaseTransform {
        private readonly ITransform _transform;

        public ShouldRunTransform(IContext context, ITransform transform) : base(context, null) {
            _transform = transform;
        }

        public override IRow Operate(IRow row) {
            return Context.Operation.ShouldRun(row) ? _transform.Operate(row) : row;
        }

        public new OperationSignature GetSignature()
        {
            throw new global::System.NotImplementedException();
        }

        public new IEnumerable<string> Errors() {
            return _transform.Errors();
        }

        public new IEnumerable<string> Warnings() {
            return _transform.Warnings();
        }
    }
}