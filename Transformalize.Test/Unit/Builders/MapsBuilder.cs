using System.Collections.Generic;

namespace Transformalize.Test.Unit.Builders {
    public class MapsBuilder {
        private List<Main.Map> _maps = new List<Main.Map>(3);

        public MapsBuilder() {
            _maps.Add(new Main.Map());
            _maps.Add(new Main.Map());
            _maps.Add(new Main.Map());
        }

        private MapBuilder Map(int index) {
            var map = new Main.Map();
            _maps[index] = map;
            var mapsBuilder = this;
            return new MapBuilder(ref mapsBuilder, map);
        }

        public MapBuilder Equals() {
            return Map(0);
        }

        public MapBuilder StartsWith() {
            return Map(1);
        }

        public MapBuilder EndsWith() {
            return Map(2);
        }

        public IEnumerable<Main.Map> ToMaps() {
            return _maps;
        }
    }
}
