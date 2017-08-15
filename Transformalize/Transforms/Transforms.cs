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
using System.Collections;
using System.Collections.Generic;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

    public class Transforms : IEnumerable<ITransform> {

        public bool Valid { get; set; }

        private readonly List<ITransform> _transforms;

        public Transforms() {
            _transforms = new List<ITransform>();
        }

        public Transforms(IEnumerable<ITransform> transforms, bool valid) {
            _transforms = new List<ITransform>(transforms);
            Valid = valid;
        }
        public IEnumerator<ITransform> GetEnumerator() {
            return _transforms.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}