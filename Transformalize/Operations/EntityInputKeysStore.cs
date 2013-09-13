using System.Linq;
using Transformalize.Main;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations
{
    public class EntityInputKeysStore : AbstractAggregationOperation
    {
        private readonly Entity _entity;
        private readonly string _firstKey;
        private readonly string[] _keys;
        private readonly Process _process;

        public EntityInputKeysStore(Process process, Entity entity)
        {
            _process = process;
            _entity = entity;
            _firstKey = _entity.PrimaryKey.First().Key;
            _keys = _entity.PrimaryKey.ToEnumerable().Select(f => f.Alias).ToArray();
        }

        protected override void Accumulate(Row row, Row aggregate)
        {
            if (aggregate.ContainsKey(_firstKey)) return;

            foreach (string key in _keys)
            {
                aggregate[key] = row[key];
            }
        }

        protected override string[] GetColumnsToGroupBy()
        {
            return _keys;
        }

        protected override void FinishAggregation(Row aggregate)
        {
            _entity.InputKeys.Add(aggregate);
        }
    }
}