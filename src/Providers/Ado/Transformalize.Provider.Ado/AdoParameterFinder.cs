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
using System.Text.RegularExpressions;
using Transformalize.Contracts;

namespace Transformalize.Providers.Ado {
    public class AdoParameterFinder : IParameterFinder {

        public static Regex Regex = new Regex("\\@([\\w.$]+)", RegexOptions.CultureInvariant | RegexOptions.Compiled);

        public IEnumerable<string> Find(string query) {
            var matches = Regex.Matches(query);
            for (var i = 0; i < matches.Count; i++) {
                var match = matches[i];
                var parameter = match.Value.TrimStart(new[] { '@' });
                yield return parameter;
            }
        }
    }
}
