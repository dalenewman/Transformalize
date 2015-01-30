using Transformalize.Main;

namespace Transformalize.Configuration.Builders {
    public class MapBuilder {
        private readonly ProcessBuilder _processBuilder;
        private readonly TflMap _map;

        public MapBuilder(ProcessBuilder processBuilder, TflMap map) {
            _processBuilder = processBuilder;
            _map = map;
        }

        public TflProcess Process() {
            return _processBuilder.Process();
        }

        public MapBuilder Connection(string name) {
            _map.Connection = name;
            return this;
        }

        public ItemBuilder Item() {
            var item = _map.GetDefaultOf<TflMapItem>();
            _map.Items.Add(item);
            return new ItemBuilder(_processBuilder, this, item);
        }

        public SearchTypeBuilder SearchType(string name) {
            return _processBuilder.SearchType(name);
        }

        public MapBuilder Sql(string sql) {
            _map.Query = sql;
            return this;
        }

        public TemplateBuilder Template(string name) {
            return _processBuilder.Template(name);
        }

        public EntityBuilder Entity(string name) {
            return _processBuilder.Entity(name);
        }
    }
}