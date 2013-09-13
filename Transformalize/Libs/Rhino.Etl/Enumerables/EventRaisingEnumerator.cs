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
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Libs.Rhino.Etl.Enumerables
{
    /// <summary>
    ///     An enumerator that will raise the events on the operation for each iterated item
    /// </summary>
    public class EventRaisingEnumerator : SingleRowEventRaisingEnumerator
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EventRaisingEnumerator" /> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="inner">The innerEnumerator.</param>
        public EventRaisingEnumerator(IOperation operation, IEnumerable<Row> inner) : base(operation, inner)
        {
        }

        /// <summary>
        ///     Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        ///     true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
        /// <filterpriority>2</filterpriority>
        public override bool MoveNext()
        {
            var result = base.MoveNext();

            if (!result)
                operation.RaiseFinishedProcessing();

            return result;
        }
    }
}