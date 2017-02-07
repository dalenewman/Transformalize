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
using System.Linq;
using System.Text;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

    public class HashcodeTransform : BaseTransform {

        readonly Field[] _input;
        readonly StringBuilder _builder;
        public HashcodeTransform(IContext context)
              : base(context, "int") {
            _input = MultipleInput();
            _builder = new StringBuilder();
        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = GetStringHashCode(_input.Select(f => row[f]));
            return row;
        }

        // Jon Skeet's Answer
        // http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
        // http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx
        public static int GetHashCode(IEnumerable<object> values) {
            unchecked {
                var hash = (int)2166136261;
                foreach (var value in values) {
                    hash = hash * 16777619 ^ value.GetHashCode();
                }
                return hash;
            }
        }

        int GetStringHashCode(IEnumerable<object> values) {
            _builder.Clear();
            foreach (var value in values) {
                _builder.Append(value ?? string.Empty);
            }
            return _builder.ToString().GetHashCode();
        }

    }
}