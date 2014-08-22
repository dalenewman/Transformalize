using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;

namespace Transformalize.Main {
    public class SearchTypeReader {
        private readonly SearchTypeElementCollection _elements;

        public SearchTypeReader(SearchTypeElementCollection elements)
        {
            _elements = elements;
        }

        public Dictionary<string, SearchType> Read()
        {
            var searchTypes = new Dictionary<string, SearchType>();

            searchTypes["none"] = new SearchType {
                Name = "none",
                Index = false,
                Store = false,
                MultiValued = false,
                Analyzer = string.Empty
            };

            searchTypes["default"] = new SearchType {
                Name = "default",
                Index = true,
                Store = true,
                MultiValued = false,
                Analyzer = string.Empty
            };

            var configuredTypes = _elements.Cast<SearchTypeConfigurationElement>().ToArray();

            if (configuredTypes.Any()) {
                foreach (var st in configuredTypes) {
                    searchTypes[st.Name] = new SearchType {
                        Index = st.Index,
                        Name = st.Name,
                        Store = st.Store,
                        MultiValued = st.MultiValued,
                        Analyzer = st.Analyzer
                    };
                }
            }
            return searchTypes;
        } 
    }
}
