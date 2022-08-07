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

    public class FromRegexTransform : StringTransform {

        private readonly Regex _regex;
        private readonly Field _input;
        private readonly Field[] _output;

        public FromRegexTransform(IContext context = null) : base(context, null) {

            ProducesFields = true;

            if(IsMissingContext()) {
                return;
            }

            if (IsNotReceiving("string")) {
                return;
            }

            if (!context.Operation.Parameters.Any()) {
                Error($"The {context.Operation.Method} transform requires a collection of output fields.");
                Run = false;
                return;
            }

            if (context.Operation.Pattern == string.Empty) {
                Error("The fromregex method requires a regular expression pattern with groups defined.");
                Run = false;
                return;
            }

#if NETS10
            _regex = new Regex(context.Operation.Pattern);
#else
            _regex = new Regex(context.Operation.Pattern, RegexOptions.Compiled);
#endif

            _input = SingleInputForMultipleOutput();
            _output = MultipleOutput();
        }

        public override IRow Operate(IRow row) {

            var match = _regex.Match(GetString(row, _input));
            if (match.Success) {
                for (var i = 0; i < match.Groups.Count && i < _output.Length; i++) {
                    var group = match.Groups[i];
                    if (!group.Success)
                        continue;
                    row[_output[i]] = _output[i].Convert(@group.Captures[0].Value);
                }
            }

            return row;

        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            return new[] { new OperationSignature("fromregex") };
        }

    }
}