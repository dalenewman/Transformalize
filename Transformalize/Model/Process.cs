using System.Collections.Generic;
using System.Linq;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Transforms;

namespace Transformalize.Model {

    public class Process {
        private Dictionary<string, Field> _fields;

        public string Name;
        public Entity MasterEntity;
        public Dictionary<string, Connection> Connections = new Dictionary<string, Connection>();
        public Dictionary<string, Entity> Entities = new Dictionary<string, Entity>();
        public List<Relationship> Relationships = new List<Relationship>();
        public Dictionary<string, HashSet<object>> KeyRegister = new Dictionary<string, HashSet<object>>();
        public Dictionary<string, Dictionary<string, object>> MapEquals = new Dictionary<string, Dictionary<string, object>>();
        public Dictionary<string, Dictionary<string, object>> MapStartsWith = new Dictionary<string, Dictionary<string, object>>();
        public Dictionary<string, Dictionary<string, object>> MapEndsWith = new Dictionary<string, Dictionary<string, object>>();
        public ITransform[] Transforms { get; set; }
        public Dictionary<string, Field> Parameters = new Dictionary<string, Field>();
        public Dictionary<string, Field> Results = new Dictionary<string, Field>();
        public IEnumerable<Field> RelatedKeys;
        public string View;

        public Dictionary<string, Field> Fields {
            get {
                if (_fields == null) {
                    _fields = new Dictionary<string, Field>();
                    foreach (var pair in Entities) {
                        foreach (var innerPair in pair.Value.All) {
                            _fields[innerPair.Key] = pair.Value.All[innerPair.Key];
                        }
                    }
                }
                return _fields;
            }
        }

        public bool HasRegisteredKey(string key) {
            return KeyRegister.ContainsKey(key);
        }

        public IEnumerable<Row> RelatedRows(string foreignKey) {
            return KeyRegister[foreignKey].Select(o => new Row { { foreignKey, o } });
        }

        public bool IsReady() {
            return Connections.Select(connection => connection.Value.IsReady()).All(b => b.Equals(true));
        }
    }
}
