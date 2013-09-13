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

using System.Collections;
using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Libs.Rhino.Etl.Enumerables
{
    /// <summary>
    ///     An enumerator that will raise the events on the operation for each iterated item
    /// </summary>
    public class SingleRowEventRaisingEnumerator : IEnumerable<Row>, IEnumerator<Row>
    {
        private readonly IEnumerable<Row> inner;

        /// <summary>
        ///     Represents the operation on which to raise events
        /// </summary>
        protected readonly IOperation operation;

        private IEnumerator<Row> innerEnumerator;
        private Row previous;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SingleRowEventRaisingEnumerator" /> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="inner">The innerEnumerator.</param>
        public SingleRowEventRaisingEnumerator(IOperation operation, IEnumerable<Row> inner)
        {
            this.operation = operation;
            this.inner = inner;
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        IEnumerator<Row> IEnumerable<Row>.GetEnumerator()
        {
            Guard.Against(inner == null, "Null enuerator detected, are you trying to read from the first operation in the process?");
            innerEnumerator = inner.GetEnumerator();
            return this;
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<Row>) this).GetEnumerator();
        }

        /// <summary>
        ///     Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <returns>
        ///     The element in the collection at the current position of the enumerator.
        /// </returns>
        public Row Current
        {
            get { return innerEnumerator.Current; }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            innerEnumerator.Dispose();
        }

        /// <summary>
        ///     Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        ///     true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
        /// <filterpriority>2</filterpriority>
        public virtual bool MoveNext()
        {
            var result = innerEnumerator.MoveNext();

            if (result)
            {
                previous = innerEnumerator.Current;
                operation.RaiseRowProcessed(Current);
            }

            return result;
        }

        /// <summary>
        ///     Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
        /// <filterpriority>2</filterpriority>
        public void Reset()
        {
            innerEnumerator.Reset();
        }

        /// <summary>
        ///     Gets the current element in the collection.
        /// </summary>
        /// <returns>
        ///     The current element in the collection.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.-or- The collection was modified after the enumerator was created.</exception>
        /// <filterpriority>2</filterpriority>
        object IEnumerator.Current
        {
            get { return innerEnumerator.Current; }
        }
    }
}