#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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
using System.Linq;
using Transformalize.Configuration;

namespace Transformalize.Impl {
    public class RelationshipSummary {

        public RelationshipSummary() {
            LeftFields = new List<Field>();
            RightFields = new List<Field>();
        }

        public Entity LeftEntity { get; set; }
        public List<Field> LeftFields { get; set; }

        public Entity RightEntity { get; set; }
        public List<Field> RightFields { get; set; }

        public bool IsAligned() {
            return LeftEntity != null && RightEntity != null && LeftFields.Any() && LeftFields.Count() == RightFields.Count();
        }

    }
}