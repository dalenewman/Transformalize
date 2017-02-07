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
using System.Threading;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.System {
    public class SetKey : BaseTransform {
        private readonly Field _tflKey;

        public SetKey(IContext context) : base(context, null) {
            _tflKey = context.Entity.TflKey();
        }

        public override IRow Transform(IRow row) {
            row[_tflKey] = Interlocked.Increment(ref Context.Entity.Identity);
            // Increment();
            return row;
        }
    }
}