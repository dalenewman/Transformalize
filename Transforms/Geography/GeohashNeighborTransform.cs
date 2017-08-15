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
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Transform.Geography {

    public class GeohashNeighborTransform : BaseTransform {
        private readonly Field _input;
        private readonly int[] _direction;

        private readonly Dictionary<string, int[]> _directions = new Dictionary<string, int[]> {
            {"north", new int[] { 1, 0}},
            {"northeast", new int[] {1,1}},
            {"east", new int[] {0, 1}},
            {"southeast", new int[] {-1, 1}},
            {"south", new int[] {-1, 0}},
            {"southwest", new int[] {-1, -1}},
            {"west", new int[] {0, -1}},
            {"northwest", new int[] {1, -1}}
        };

        public GeohashNeighborTransform(IContext context) : base(context, "string") {
            if (IsNotReceiving("string")) {
                return;
            }

            _input = SingleInput();
            _direction = _directions[context.Transform.Direction];
        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = NGeoHash.Portable.GeoHash.Neighbor((string)row[_input], _direction);
            Increment();
            return row;
        }
    }
}