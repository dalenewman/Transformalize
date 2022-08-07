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
using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.Geography {

    public class GeohashNeighborTransform : BaseTransform {

        private readonly Field _input;
        private readonly int[] _direction;

        private readonly Dictionary<string, int[]> _directions = new Dictionary<string, int[]> {
            {"north", new int[] { 1, 0}},
            {"asc", new int[] { 1, 0}},
            {"northeast", new int[] {1,1}},
            {"east", new int[] {0, 1}},
            {"southeast", new int[] {-1, 1}},
            {"south", new int[] {-1, 0}},
            {"desc", new int[] {-1, 0}},
            {"southwest", new int[] {-1, -1}},
            {"west", new int[] {0, -1}},
            {"northwest", new int[] {1, -1}}
        };

        public GeohashNeighborTransform(IContext context = null) : base(context, "string") {

            if (IsMissingContext()){
                return;
            }

            if (IsNotReceiving("string")) {
                return;
            }

            _input = SingleInput();
            _direction = _directions[Context.Operation.Direction];
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = NGeoHash.GeoHash.Neighbor((string)row[_input], _direction);
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            return new[] {
                new OperationSignature("geohashneighbor") {
                    Parameters = new List<OperationParameter>(1) {
                        new OperationParameter("direction")
                    }
                }
            };
        }
    }
}