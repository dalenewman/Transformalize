using System.Linq;
using Transformalize.Core.Entity_;
using Transformalize.Core.Process_;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations {
    public class EntityInputKeysStore : AbstractAggregationOperation {
        private readonly Entity _entity;
        private readonly string _firstKey;
        private readonly string[] _keys;

        public EntityInputKeysStore(Entity entity) {
            _entity = entity;
            _firstKey = _entity.PrimaryKey.First().Key;
            _keys = _entity.PrimaryKey.ToEnumerable().Select(f=>f.Alias).ToArray();
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

        protected override void FinishAggregation(Row aggregate)
        {
            if (Process.OutputRecordsExist)
            {
                _entity.InputKeys.Add(aggregate);
            }
        }
    }
}