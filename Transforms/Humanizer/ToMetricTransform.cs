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
using System;
using Humanizer;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Transforms;

namespace Transformalize.Transform.Humanizer {
    public class ToMetricTransform : BaseTransform {
        private readonly Func<IRow, object> _transform;
        private readonly Field _input;

        public ToMetricTransform(IContext context) : base(context, "string") {
            Run = HasValidNumericInput();
            if (!Run) {
                return;
            }
            _input = SingleInput();
            switch (_input.Type.Left(3)) {
                case "int":
                case "sho":
                    _transform = (row) => {
                        var input = _input.Type.In("int", "int32") ? (int)row[_input] : Convert.ToInt32(row[_input]);
                        return input.ToMetric();
                    };
                    break;
                case "double":
                    _transform = (row) => {
                        var input = (double)row[_input];
                        return input.ToMetric();
                    };
                    break;
                default:
                    _transform = (row) => row[_input];
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