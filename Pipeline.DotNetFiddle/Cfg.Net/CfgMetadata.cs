#region license
// Cfg.Net
// Copyright 2015 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cfg.Net {
    sealed class CfgMetadata {


        private readonly HashSet<string> _domainSet;
        private readonly HashSet<string> _validatorSet;

        public PropertyInfo PropertyInfo { get; set; }
        public CfgAttribute Attribute { get; set; }
        public Type ListType { get; set; }
        public Func<CfgNode> Loader { get; set; }
        public string[] UniquePropertiesInList { get; set; }
        public Action<object, object> Setter { get; set; }
        public Func<object, object> Getter { get; set; }
        public bool TypeMismatch { get; set; }
        public object Default { get; set; }

        public CfgMetadata(PropertyInfo propertyInfo, CfgAttribute attribute) {
            PropertyInfo = propertyInfo;
            Attribute = attribute;

            if (!string.IsNullOrEmpty(attribute.domain)) {
                var comparer = attribute.ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
                _domainSet = new HashSet<string>(attribute.domain.Split(new[] { attribute.delimiter }, StringSplitOptions.None).Distinct(), comparer);
            }

            if (string.IsNullOrEmpty(attribute.validators)) return;

            _validatorSet =
                new HashSet<string>(
                    attribute.validators.Split(new[] { attribute.delimiter }, StringSplitOptions.None).Distinct(),
                    attribute.ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
        }

        public bool IsInDomain(string value) {
            return _domainSet == null || (value != null && _domainSet.Contains(value));
        }

        public IEnumerable<string> Validators() {
            return _validatorSet ?? (IEnumerable<string>)new string[0];
        }
    }
}