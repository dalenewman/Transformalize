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
using System.Threading;
using Transformalize.Libs.Rhino.Etl.Enumerables;

namespace Transformalize.Libs.Rhino.Etl.Operations
{
    /// <summary>
    ///     Branch the current pipeline flow into all its inputs
    /// </summary>
    public class MultiThreadedBranchingOperation : AbstractBranchingOperation
    {
        /// <summary>
        ///     Executes this operation
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            var input = new GatedThreadSafeEnumerator<Row>(Operations.Count, rows);

            var sync = new object();

            foreach (var operation in Operations)
            {
                var clone = input.Select(r => r.Clone());
                var result = operation.Execute(clone);

                if (result == null)
                {
                    input.Dispose();
                    continue;
                }

                var enumerator = result.GetEnumerator();

                ThreadPool.QueueUserWorkItem(delegate
                                                 {
                                                     try
                                                     {
                                                         while (enumerator.MoveNext()) ;
                                                     }
                                                     finally
                                                     {
                                                         lock (sync)
                                                         {
                                                             enumerator.Dispose();
                                                             Monitor.Pulse(sync);
                                                         }
                                                     }
                                                 });
            }

            lock (sync)
                while (input.ConsumersLeft > 0)
                    Monitor.Wait(sync);

            yield break;
        }
    }
}