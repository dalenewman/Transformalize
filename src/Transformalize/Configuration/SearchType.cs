#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2026 Dale Newman
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

namespace Transformalize.Configuration {
    public class SearchType : CfgNode {
        [Cfg(required = true, unique = true, toLower = true)]
        public string Name { get; set; }

        [Cfg(value = true)]
        public bool Store { get; set; }

        [Cfg(value = true)]
        public bool Index { get; set; }

        [Cfg(value = false)]
        public bool MultiValued { get; set; }

        [Cfg(value = "")]
        public string Analyzer { get; set; }

        [Cfg(value = true)]
        public bool Norms { get; set; }

        [Cfg(value = "defer")]
        public string Type { get; set; }

        /// <summary>
        /// Controls the FTS predicate used by the provider.
        /// SQL Server: "freetext" → FREETEXT(); anything else (default) → CONTAINS() with auto-normalizer.
        /// PostgreSQL: "plain" (plainto_tsquery), "web" (websearch_to_tsquery),
        ///   "phrase" (phraseto_tsquery), "raw" (to_tsquery).
        /// </summary>
        [Cfg(value = "plain", domain="plain,web,phrase,raw,contains,freetext", toLower = true)]
        public string QueryType { get; set; }

        /// <summary>
        /// For MySQL full-text search: the AGAINST mode.
        /// Options: "boolean" (IN BOOLEAN MODE), "natural" (IN NATURAL LANGUAGE MODE),
        /// "expansion" (WITH QUERY EXPANSION)
        /// </summary>
        [Cfg(value = "boolean", domain="boolean,natural,expansion", toLower = true)]
        public string Mode { get; set; }
    }
}