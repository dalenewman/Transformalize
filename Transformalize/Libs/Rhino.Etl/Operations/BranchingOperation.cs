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
using Transformalize.Libs.Rhino.Etl.Enumerables;

namespace Transformalize.Libs.Rhino.Etl.Operations
{
    /// <summary>
    ///     Branch the current pipeline flow into all its inputs
    /// </summary>
    public class BranchingOperation : AbstractBranchingOperation
    {
        /// <summary>
        ///     Executes this operation, sending the input of this operation
        ///     to all its child operations
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            var copiedRows = new CachingEnumerable<Row>(rows);

            foreach (var operation in Operations)
            {
                var cloned = copiedRows.Select(r => r.Clone());

                var enumerable = operation.Execute(cloned);

                if (enumerable == null)
                    continue;

                var enumerator = enumerable.GetEnumerator();
#pragma warning disable 642
                while (enumerator.MoveNext()) ;
#pragma warning restore 642
            }
            yield break;
        }
    }
}