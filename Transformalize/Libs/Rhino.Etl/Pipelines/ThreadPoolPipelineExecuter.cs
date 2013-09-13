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

using System;
using System.Collections.Generic;
using System.Threading;
using Transformalize.Libs.Rhino.Etl.Enumerables;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Libs.Rhino.Etl.Pipelines
{
    /// <summary>
    ///     Execute all the actions concurrently, in the thread pool
    /// </summary>
    public class ThreadPoolPipelineExecuter : AbstractPipelineExecuter
    {
        /// <summary>
        ///     Add a decorator to the enumerable for additional processing
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="enumerator">The enumerator.</param>
        protected override IEnumerable<Row> DecorateEnumerableForExecution(IOperation operation, IEnumerable<Row> enumerator)
        {
            var threadedEnumerator = new ThreadSafeEnumerator<Row>();
            ThreadPool.QueueUserWorkItem(delegate
                                             {
                                                 try
                                                 {
                                                     foreach (Row t in new EventRaisingEnumerator(operation, enumerator))
                                                     {
                                                         threadedEnumerator.AddItem(t);
                                                     }
                                                 }
                                                 catch (Exception e)
                                                 {
                                                     Error("Failed to execute {0}. {1} {2}", operation.Name, e.Message, e.InnerException != null ? e.InnerException.Message : string.Empty);
                                                     Environment.Exit(0);
                                                     threadedEnumerator.MarkAsFinished();
                                                 }
                                                 finally
                                                 {
                                                     threadedEnumerator.MarkAsFinished();
                                                 }
                                             });
            return threadedEnumerator;
        }
    }
}