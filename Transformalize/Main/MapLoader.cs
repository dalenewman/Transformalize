using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Main.Providers;

namespace Transformalize.Main {
    public class MapLoader {

        private readonly Process _process;
        private readonly List<TflMap> _elements;

        public MapLoader(ref Process process, List<TflMap> elements) {
            _process = process;
            _elements = elements;
        }

        public void Load() {
            foreach (var m in _elements) {
                if (string.IsNullOrEmpty(m.Query)) {
                    _process.MapEquals[m.Name] = new MapConfigurationReader(m.Items, "equals").Read();
                    _process.MapStartsWith[m.Name] = new MapConfigurationReader(m.Items, "startswith").Read();
                    _process.MapEndsWith[m.Name] = new MapConfigurationReader(m.Items, "endswith").Read();
                } else {
                    var connection = _process.Connections.GetConnectionByName(m.Connection);
                    _process.MapEquals[m.Name] = new SqlMapReader(m.Query, connection.Connection).Read();
                }
            }
        }
    }
}
