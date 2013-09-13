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

using System.Collections.Generic;
using System.Linq;

namespace Transformalize.Libs.Rhino.Etl.Operations
{
    public class DistinctOperation : AbstractAggregationOperation
    {
        private readonly IEnumerable<string> _columnsToGroupBy;

        public DistinctOperation(IEnumerable<string> columnsToGroupBy)
        {
            _columnsToGroupBy = columnsToGroupBy;
        }

        protected override void Accumulate(Row row, Row aggregate)
        {
            foreach (var column in _columnsToGroupBy)
            {
                aggregate[column] = row[column];
            }
        }

        protected override string[] GetColumnsToGroupBy()
        {
            return _columnsToGroupBy.ToArray();
        }
    }
}