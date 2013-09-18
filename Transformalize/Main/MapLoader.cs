using System;
using Transformalize.Configuration;
using Transformalize.Libs.NLog;
using Transformalize.Main.Providers;

namespace Transformalize.Main {
    public class MapLoader {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly Process _process;
        private readonly MapElementCollection _elements;

        public MapLoader(ref Process process, MapElementCollection elements) {
            _process = process;
            _elements = elements;
        }

        public void Load() {
            foreach (MapConfigurationElement m in _elements) {
                if (string.IsNullOrEmpty(m.Connection)) {
                    _process.MapEquals[m.Name] = new MapConfigurationReader(m.Items, "equals").Read();
                    _process.MapStartsWith[m.Name] = new MapConfigurationReader(m.Items, "startswith").Read();
                    _process.MapEndsWith[m.Name] = new MapConfigurationReader(m.Items, "endswith").Read();
                } else {
                    if (_process.Connections.ContainsKey(m.Connection)) {
                        _process.MapEquals[m.Name] = new SqlMapReader(m.Items.Sql, _process.Connections[m.Connection]).Read();
                    } else {
                        _log.Error("Map {0} references connection {1}, which does not exist.", m.Name, m.Connection);
                        Environment.Exit(0);
                    }
                }
            }
        }
    }
}
