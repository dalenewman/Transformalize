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

using System.Text.RegularExpressions;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Validators {

    /// <summary>
    /// returns true if matches any of the input
    /// </summary>
    public class MatchValidator : StringValidate {
        private readonly Regex _regex;
        private readonly Field _input;

        public MatchValidator(IContext context) : base(context) {

            if (!Run)
                return;

            _input = SingleInput();
#if NETS10
            _regex = new Regex(context.Operation.Pattern);
#else
            _regex = new Regex(context.Operation.Pattern, RegexOptions.Compiled);
#endif
        }

        public override IRow Operate(IRow row) {

            var match = _regex.Match(GetString(row, _input));
            row[ValidField] = match.Success;

            if (!match.Success) {
                AppendMessage(row, $"Must match pattern {Context.Operation.Pattern}.");
            }

            Increment();
            return row;
        }
    }
}