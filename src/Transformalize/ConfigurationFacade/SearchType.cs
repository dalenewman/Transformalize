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

namespace Transformalize.ConfigurationFacade
{
    public class SearchType : CfgNode
    {
        [Cfg]
        public string Name { get; set; }

        [Cfg]
        public string Store { get; set; }

        [Cfg]
        public string Index { get; set; }

        [Cfg]
        public string MultiValued { get; set; }

        [Cfg]
        public string Analyzer { get; set; }

        [Cfg]
        public string Norms { get; set; }

        [Cfg]
        public string Type { get; set; }

        [Cfg]
        public string QueryType { get; set; }

        [Cfg]
        public string Mode { get; set; }

        public Configuration.SearchType ToSearchType()
        {
            var st = new Configuration.SearchType
            {
                Name = this.Name,
                Analyzer = this.Analyzer,
                Type = this.Type,
                QueryType = this.QueryType,
                Mode = this.Mode
            };

            bool.TryParse(this.Store, out bool store);
            st.Store = store;

            bool.TryParse(this.Index, out bool index);
            st.Index = index;

            bool.TryParse(this.MultiValued, out bool multiValued);
            st.MultiValued = multiValued;

            bool.TryParse(this.Norms, out bool norms);
            st.Norms = norms;

            return st;
        }

    }
}