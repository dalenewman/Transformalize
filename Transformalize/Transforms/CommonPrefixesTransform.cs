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
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class CommonPrefixesTransform : BaseTransform {
        private readonly Field[] _input;
        private readonly string _sep;

        public CommonPrefixesTransform(IContext context) : base(context, "string") {
            _input = MultipleInput();
            _sep = Context.Operation.Separator == Constants.DefaultSetting ? "," : Context.Operation.Separator;
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = string.Join(_sep, Get(_input.Select(f => f.Type == "string" ? (string)row[f] : row[f].ToString()).ToArray()));
            return row;
        }

        public static IEnumerable<string> Get(string[] input) {

            var prefix = CommonPrefixTransform.Get(input);

            if (prefix != string.Empty)
                return new[] { prefix };

            var letters = input.Where(s=>s.Length > 0).Select(s => s[0]).Distinct();

            return letters.Select(letter => input.Where(s => s.Length > 0 && s[0] == letter).ToArray()).Select(CommonPrefixTransform.Get);
        }
    }
}