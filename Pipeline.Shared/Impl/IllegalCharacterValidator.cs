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
using Cfg.Net.Contracts;

namespace Transformalize.Impl {

    public class IllegalCharacterValidator : ICustomizer {

        private readonly string _illegalCharacters;
        private HashSet<char> _illegal;
        private HashSet<char> Illegal => _illegal ?? (_illegal = new HashSet<char>(_illegalCharacters.ToCharArray()));

        public IllegalCharacterValidator(string illegalCharacters = ";'`") {
            _illegalCharacters = illegalCharacters;
        }

        public void Customize(string parent, INode node, IDictionary<string, string> parameters, ILogger logger) {

            if (parent != "parameters")
                return;

            IAttribute attr;
            if (!node.TryAttribute("value", out attr))
                return;

            var value = attr.Value.ToString();
            if (!string.IsNullOrEmpty(value) && value.ToCharArray().Any(c => Illegal.Contains(c))) {
                IAttribute nameAttr;
                node.TryAttribute("name", out nameAttr);
                logger.Error($"The parameter {(nameAttr == null ? "?" : nameAttr.Value)} contains an illegal character (e.g. {string.Join(",", Illegal)}).");
            }
        }

        public void Customize(INode root, IDictionary<string, string> parameters, ILogger logger) { }
    }
}
