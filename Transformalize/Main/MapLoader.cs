using System.Collections.Generic;
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
                    if (_process.Connections.ContainsKey(m.Connection)) {
                        _process.MapEquals[m.Name] = new SqlMapReader(m.Query, _process.Connections[m.Connection]).Read();
                    } else {
                        throw new TransformalizeException("Map {0} references connection {1}, which does not exist.", m.Name, m.Connection);
                    }
                }
            }
        }
    }
}
