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
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class CommonPrefixTransform : BaseTransform {
        private readonly Field[] _input;

        public CommonPrefixTransform(IContext context) : base(context, "string") {
            _input = MultipleInput();
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = Get(_input.Select(f => f.Type == "string" ? (string)row[f] : row[f].ToString()).ToArray());
            Increment();
            return row;
        }

        public static string Get(string[] input) {

            switch (input.Length) {
                case 0:
                    return string.Empty;
                case 1:
                    return input[0];
            }

            var prefixLength = 0;

            foreach (var c in input[0]) {
                if (input.Any(s => s.Length <= prefixLength || s[prefixLength] != c)) {
                    return input[0].Substring(0, prefixLength);
                }
                prefixLength++;
            }

            return input[0];
        }
    }
}