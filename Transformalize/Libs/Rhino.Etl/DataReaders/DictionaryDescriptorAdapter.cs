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
    ///     Adapts a dictionary to a descriptor
    /// </summary>
    public class DictionaryDescriptorAdapter : Descriptor
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DictionaryDescriptorAdapter" /> class.
        /// </summary>
        /// <param name="pair">The pair.</param>
        public DictionaryDescriptorAdapter(KeyValuePair<string, Type> pair)
            : base(pair.Key, pair.Value)
        {
        }

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public override object GetValue(object obj)
        {
            return ((Row) obj)[Name];
        }
    }
}