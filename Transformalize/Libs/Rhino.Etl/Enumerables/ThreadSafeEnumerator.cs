#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Transformalize.Libs.Rhino.Etl.Enumerables
{
    /// <summary>
    ///     This enumerator allows to safely move items between threads. It takes
    ///     care of all the syncronization.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ThreadSafeEnumerator<T> : IEnumerable<T>, IEnumerator<T>
    {
        private readonly Queue<T> _cached = new Queue<T>();
        private bool _active = true;
        private T _current;

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>) this).GetEnumerator();
        }

        /// <summary>
        ///     Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value></value>
        /// <returns>The element in the collection at the current position of the enumerator.</returns>
        public T Current
        {
            get { return _current; }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _cached.Clear();
        }

        /// <summary>
        ///     Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        ///     true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
        public bool MoveNext()
        {
            lock (_cached)
            {
                while (_cached.Count == 0 && _active)
                    Monitor.Wait(_cached);

                if (_active == false && _cached.Count == 0)
                    return false;

                _current = _cached.Dequeue();

                return true;
            }
        }

        /// <summary>
        ///     Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
        public void Reset()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value></value>
        /// <returns>The element in the collection at the current position of the enumerator.</returns>
        object IEnumerator.Current
        {
            get { return Current; }
        }

        /// <summary>
        ///     Adds the item to the items this is enumerating on.
        ///     Will immediately release a waiting thread that can start working on itl
        /// </summary>
        /// <param name="item">The item.</param>
        public void AddItem(T item)
        {
            lock (_cached)
            {
                _cached.Enqueue(item);
                Monitor.Pulse(_cached);
            }
        }

        /// <summary>
        ///     Marks this instance as finished, so it will stop iterating
        /// </summary>
        public void MarkAsFinished()
        {
            lock (_cached)
            {
                _active = false;
                Monitor.Pulse(_cached);
            }
        }
    }
}