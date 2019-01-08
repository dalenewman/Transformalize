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
using System.Linq;
using System.Text.RegularExpressions;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class RegexMatchingTransform : StringTransform {

        private readonly Field _input;
        private readonly Regex _regex;
        private readonly string _default;

        public RegexMatchingTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext()) {
                return;
            }

            if (IsNotReceiving("string")) {
                return;
            }

            if (IsMissing(Context.Operation.Pattern)) {
                return;
            }

            _default = Context.Field.Default == Constants.DefaultSetting ? Constants.StringDefaults()[Context.Field.Type] : Context.Field.Convert(Context.Field.Default).ToString();

            _input = SingleInput();

#if NETS10
            _regex = new Regex(Context.Operation.Pattern);
#else
            _regex = new Regex(Context.Operation.Pattern, RegexOptions.Compiled);
#endif

        }

        public override IRow Operate(IRow row) {
            var matches = _regex.Matches(GetString(row, _input));
            row[Context.Field] = matches.Count > 0 ? string.Concat(matches.Cast<Match>().Select(m => m.Value)) : _default;
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            return new[] {
                new OperationSignature("matching") {
                    Parameters =  new List<OperationParameter> {
                        new OperationParameter("pattern")
                    }
                }
            };
        }
    }
}