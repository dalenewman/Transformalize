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
using System;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    public class FloorTransform : BaseTransform {

        readonly Field _input;
        private readonly Func<IRow, object> _transform;

        public FloorTransform(IContext context) : base(context, "decimal") {
            _input = SingleInput();
            switch (_input.Type) {
                case "decimal":
                    Returns = "decimal";
                    _transform = row => Math.Floor((decimal)row[_input]);
                    break;
                case "double":
                    Returns = "double";
                    _transform = row => Math.Floor((double)row[_input]);
                    break;
                default:
                    Returns = "decimal";
                    _transform = row => Math.Floor(Convert.ToDecimal(row[_input]));
                    break;
            }
        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = _transform(row);
            Increment();
            return row;
        }
    }
}