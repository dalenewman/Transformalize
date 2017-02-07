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
using Cfg.Net;
using Transformalize.Contracts;

namespace Transformalize.Configuration {
    public class Filter : CfgNode, IHasField {
        string _continuation;

        /// <summary>
        /// Optional.  Default is `And`
        /// 
        /// A continuation operator.  Valid values are:
        /// 
        /// * And
        /// * Or
        /// </summary>
        [Cfg(value = "And", domain = "And,Or", ignoreCase = true)]
        public string Continuation {
            get { return _continuation; }
            set {
                if (value != null)
                    _continuation = value.ToUpper();
            }
        }

        /// <summary>
        /// Optional
        /// 
        /// A free-form, un-checked expression.  This is passed directly into a generated query.
        /// </summary>
        [Cfg(value = "")]
        public string Expression { get; set; }

        /// <summary>
        /// Optional
        /// 
        /// A reference to an entity field's name\alias or a literal on the left side of an expression.
        /// </summary>
        [Cfg(value = "")]
        public string Field { get; set; }

        /// <summary>
        /// Optional
        /// 
        /// A reference to an entity field's name\alias or a literal on the right side of an expression.
        /// </summary>
        [Cfg(value = "")]
        public string Value { get; set; }

        /// <summary>
        /// Optional.  Default is `equal`
        /// 
        /// A comparison operator.
        /// </summary>
        [Cfg(value = "equal", domain = Constants.ComparisonDomain, toLower = true, ignoreCase = true)]
        public string Operator { get; set; }

        public bool IsField { get; set; }
        public bool ValueIsField { get; set; }
        public Field LeftField { get; set; }
        public Field ValueField { get; set; }

        [Cfg(value="search",domain="search,facet,range", ignoreCase = true, toLower = true)]
        public string Type { get; set; }

        [Cfg(value=100)]
        public int Size { get; set; }

        [Cfg(value="_term")]
        public string OrderBy { get; set; }

        [Cfg(value="asc", domain="asc,desc", toLower = true)]
        public string Order { get; set; }

        [Cfg(value=1)]
        public int Min { get; set; }

        public string Key { get; set; }

        [Cfg(value="", toLower = true)]
        public string Map { get; set; }
    }
}