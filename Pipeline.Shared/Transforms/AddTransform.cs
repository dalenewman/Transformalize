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
using System.Linq;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    public class AddTransform : BaseTransform {
        readonly Field[] _input;
        private readonly Func<IRow, object> _transform;

        public AddTransform(IContext context) : base(context, "decimal") {
            _input = MultipleInput();
            var same = _input.All(i => i.Type == _input.First().Type);
            if (same) {
                var type = _input.First().Type;
                switch (type) {
                    case "decimal":
                        Returns = "decimal";
                        _transform = row => _input.Sum(f => (decimal)row[f]);
                        break;
                    case "double":
                        Returns = "double";
                        _transform = row => _input.Sum(f => (double)row[f]);
                        break;
                    case "long":
                    case "int64":
                        Returns = "long";
                        _transform = row => _input.Sum(f => (long)row[f]);
                        break;
                    case "int":
                    case "int32":
                        Returns = "int";
                        _transform = row => _input.Sum(f => (int)row[f]);
                        break;
                    //case "float":
                    //    Returns = "float";
                    //    _transform = row => _input.Sum(f => (float) row[f]);
                    //    break;
                    default:
                        _transform = row => _input.Sum(f => Convert.ToDecimal(row[f]));
                        break;
                }
            } else {
                _transform = row => _input.Sum(field => field.Type == "decimal" ? (decimal)row[field] : Convert.ToDecimal(row[field]));
            }
        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = _transform(row);
            Increment();
            return row;
        }
    }
}