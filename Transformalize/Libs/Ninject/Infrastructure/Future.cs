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

#region Using Directives

using System;

#endregion

namespace Transformalize.Libs.Ninject.Infrastructure
{
    /// <summary>
    ///     Represents a future value.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    public class Future<T>
    {
        private bool _hasValue;
        private T _value;

        /// <summary>
        ///     Initializes a new instance of the Future&lt;T&gt; class.
        /// </summary>
        /// <param name="callback">The callback that will be triggered to read the value.</param>
        public Future(Func<T> callback)
        {
            Callback = callback;
        }

        /// <summary>
        ///     Gets the value, resolving it if necessary.
        /// </summary>
        public T Value
        {
            get
            {
                if (!_hasValue)
                {
                    _value = Callback();
                    _hasValue = true;
                }

                return _value;
            }
        }

        /// <summary>
        ///     Gets the callback that will be called to resolve the value.
        /// </summary>
        public Func<T> Callback { get; private set; }

        /// <summary>
        ///     Gets the value from the future.
        /// </summary>
        /// <param name="future">The future.</param>
        /// <returns>The future value.</returns>
        public static implicit operator T(Future<T> future)
        {
            return future.Value;
        }
    }
}