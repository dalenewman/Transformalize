using System.Linq;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations {
    public class EntityInputKeysStore : AbstractAggregationOperation {
        private readonly Entity _entity;
        private readonly string _firstKey;
        private readonly string[] _keys;

        public EntityInputKeysStore(Entity entity) {
            _entity = entity;
            _firstKey = _entity.PrimaryKey.Select(k => k.Key).First();
            _keys = _entity.PrimaryKey.Select(k => k.Key).ToArray();
        }

        protected override void Accumulate(Row row, Row aggregate) {

            if (aggregate.ContainsKey(_firstKey)) return;

            foreach (var pair in _entity.PrimaryKey) {
                aggregate[pair.Key] = row[pair.Key];
            }
        }

        protected override string[] GetColumnsToGroupBy() {
            return _keys;
        }
    }
}