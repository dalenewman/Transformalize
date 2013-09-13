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

namespace Transformalize.Libs.Rhino.Etl.Operations
{
    /// <summary>
    ///     An aggregation operation, handles all the backend stuff of the aggregation,
    ///     leaving client code just the accumulation process
    /// </summary>
    public abstract class AbstractAggregationOperation : AbstractOperation
    {
        /// <summary>
        ///     Executes this operation
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            IDictionary<ObjectArrayKeys, Row> aggregations = new Dictionary<ObjectArrayKeys, Row>();
            var groupBy = GetColumnsToGroupBy();
            foreach (var row in rows)
            {
                var key = row.CreateKey(groupBy);
                Row aggregate;
                if (aggregations.TryGetValue(key, out aggregate) == false)
                    aggregations[key] = aggregate = new Row();
                Accumulate(row, aggregate);
            }
            foreach (var row in aggregations.Values)
            {
                FinishAggregation(row);
                yield return row;
            }
        }

        /// <summary>
        ///     Allow a derived class to perform final processing on the
        ///     aggregate, before sending it downward in the pipeline.
        /// </summary>
        /// <param name="aggregate">The row.</param>
        protected virtual void FinishAggregation(Row aggregate)
        {
        }

        /// <summary>
        ///     Accumulate the current row to the current aggregation
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="aggregate">The aggregate.</param>
        protected abstract void Accumulate(Row row, Row aggregate);

        /// <summary>
        ///     Gets the columns list to group each row by
        /// </summary>
        /// <value>The group by.</value>
        protected virtual string[] GetColumnsToGroupBy()
        {
            return new string[0];
        }
    }
}