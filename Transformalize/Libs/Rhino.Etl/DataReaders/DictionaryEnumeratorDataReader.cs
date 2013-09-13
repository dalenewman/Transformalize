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

namespace Transformalize.Libs.Rhino.Etl.DataReaders
{
    /// <summary>
    ///     A datareader over a collection of dictionaries
    /// </summary>
    public class DictionaryEnumeratorDataReader : EnumerableDataReader
    {
        private readonly IEnumerable<Row> _enumerable;
        private readonly List<Descriptor> _propertyDescriptors = new List<Descriptor>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="DictionaryEnumeratorDataReader" /> class.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="enumerable">The enumerator.</param>
        public DictionaryEnumeratorDataReader(IEnumerable<KeyValuePair<string, Type>> schema, IEnumerable<Row> enumerable)
            : base(enumerable.GetEnumerator())
        {
            _enumerable = enumerable;
            foreach (var pair in schema)
            {
                _propertyDescriptors.Add(new DictionaryDescriptorAdapter(pair));
            }
        }

        /// <summary>
        ///     Gets the descriptors for the properties that this instance
        ///     is going to handle
        /// </summary>
        /// <value>The property descriptors.</value>
        protected override IList<Descriptor> PropertyDescriptors
        {
            get { return _propertyDescriptors; }
        }

        /// <summary>
        ///     Perform the actual closing of the reader
        /// </summary>
        protected override void DoClose()
        {
            var disposable = Enumerator as IDisposable;
            if (disposable != null)
                disposable.Dispose();

            disposable = _enumerable as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
    }
}