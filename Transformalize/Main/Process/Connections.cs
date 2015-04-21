using System.Collections;
using System.Collections.Generic;
using Transformalize.Configuration;

namespace Transformalize.Main {

    public class Connections : IEnumerable<TflConnection> {

        private readonly Dictionary<string, TflConnection> _connections = new Dictionary<string, TflConnection>();

        public void AddRange(IEnumerable<TflConnection> connections) {
            foreach (var connection in connections) {
                Add(connection);
            }
        }

        public void Add(TflConnection connection) {
            _connections[connection.Name] = connection;
        }

        public TflConnection GetConnectionByName(string name) {
            return _connections[name];
        }

        public bool Contains(string name) {
            return _connections.ContainsKey(name);
        }

        public TflConnection Output() {
            return _connections["output"];
        }

        public IEnumerator<TflConnection> GetEnumerator() {
            return _connections.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}