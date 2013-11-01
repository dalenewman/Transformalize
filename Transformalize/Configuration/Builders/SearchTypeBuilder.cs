namespace Transformalize.Configuration.Builders {
    public class SearchTypeBuilder {
        private readonly ProcessBuilder _processBuilder;
        private readonly SearchTypeConfigurationElement _searchType;

        public SearchTypeBuilder(ProcessBuilder processBuilder, SearchTypeConfigurationElement searchType) {
            _processBuilder = processBuilder;
            _searchType = searchType;
        }

        public SearchTypeBuilder Type(string type) {
            _searchType.Type = type;
            return this;
        }

        public SearchTypeBuilder MultiValued(bool multiValued) {
            _searchType.MultiValued = multiValued;
            return this;
        }

        public SearchTypeBuilder Store(bool store) {
            _searchType.Store = store;
            return this;
        }

        public SearchTypeBuilder Index(bool index) {
            _searchType.Index = index;
            return this;
        }

        public SearchTypeBuilder SearchType(string name) {
            return _processBuilder.SearchType(name);
        }

        public EntityBuilder Entity(string name) {
            return _processBuilder.Entity(name);
        }
    }
}