#region license
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
    public class FormatPhoneTransform : StringTransform {

        private readonly Field _input;
#if NETS10
        private readonly Regex _clean = new Regex("[^0-9]");
#else
        private readonly Regex _clean = new Regex("[^0-9]", RegexOptions.Compiled);
#endif

        public FormatPhoneTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext()) {
                return;
            }
            _input = SingleInput();
        }

        public override IRow Operate(IRow row) {
            var clean = _clean.Replace(GetString(row, _input), string.Empty);
            if (clean.Length == 10) {
                row[Context.Field] = $"({clean.Substring(0, 3)}) {clean.Substring(3, 3)}-{clean.Substring(6, 4)}";
            } else {
                row[Context.Field] = clean;
            }
            
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            return new[] { new OperationSignature("formatphone") };
        }
    }
}