﻿#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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
using System.Text.RegularExpressions;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class RegexMatchCountTransform : BaseTransform {

        private readonly Regex _regex;
        private readonly Field[] _input;

        public RegexMatchCountTransform(IContext context = null) : base(context, "int") {
            if (IsMissingContext()) {
                return;
            }

            if (IsNotReceiving("string")) {
                return;
            }

            if (IsMissing(Context.Operation.Pattern)) {
                return;
            }

            _input = MultipleInput();
#if NETS10
            _regex = new Regex(Context.Operation.Pattern);
#else
            _regex = new Regex(Context.Operation.Pattern, RegexOptions.Compiled);
#endif
        }

        public override IRow Operate(IRow row) {
            int count = 0;
            foreach (var field in _input) {
                var matches = _regex.Matches(row[field].ToString());
                count += matches.Count;
            }
            row[Context.Field] = count;

            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("matchcount") { Parameters = new List<OperationParameter>(1) { new OperationParameter("pattern") } };
        }
    }
}