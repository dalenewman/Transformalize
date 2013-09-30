#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations
{
    public class EntityInputKeysStore : AbstractAggregationOperation
    {
        private readonly Entity _entity;
        private readonly string _firstKey;
        private readonly string[] _keys;

        public EntityInputKeysStore(Entity entity)
        {
            _entity = entity;
            _firstKey = _entity.PrimaryKey.First().Key;
            _keys = _entity.PrimaryKey.ToEnumerable().Select(f => f.Alias).ToArray();
        }

        protected override void Accumulate(Row row, Row aggregate)
        {
            if (aggregate.ContainsKey(_firstKey)) return;

            foreach (var key in _keys)
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