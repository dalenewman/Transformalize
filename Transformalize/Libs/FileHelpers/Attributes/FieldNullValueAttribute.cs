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

namespace Transformalize.Libs.FileHelpers.Attributes
{
    /// <summary>Indicates the value to assign to the field in the case of find a "NULL".</summary>
    /// <remarks>
    ///     See the <a href="attributes.html">Complete Attributes List</a> for more clear info and examples of each one.
    /// </remarks>
    /// <seealso href="attributes.html">Attributes List</seealso>
    /// <seealso href="quick_start.html">Quick Start Guide</seealso>
    /// <seealso href="examples.html">Examples of Use</seealso>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FieldNullValueAttribute : Attribute
    {
        internal object NullValue;
//		internal bool NullValueOnWrite = false;


        /// <summary>Indicates directly the null value.</summary>
        /// <param name="nullValue">The value to assign in the "NULL" case.</param>
        public FieldNullValueAttribute(object nullValue)
        {
            NullValue = nullValue;
//			NullValueOnWrite = useOnWrite;
        }

//		/// <summary>Indicates directly the null value.</summary>
//		/// <param name="nullValue">The value to assign in the "NULL" case.</param>
//		public FieldNullValueAttribute(object nullValue): this(nullValue, false)
//		{}

//		/// <summary>Indicates a type and a string to be converted to that type.</summary>
//		/// <param name="type">The type of the null value.</param>
//		/// <param name="nullValue">The string to be converted to the specified type.</param>
//		/// <param name="useOnWrite">Indicates that if the field has that value when the library writes, then the engine use an empty string.</param>
//		public FieldNullValueAttribute(Type type, string nullValue, bool useOnWrite):this(Convert.ChangeType(nullValue, type, null), useOnWrite)
//		{}

        /// <summary>Indicates a type and a string to be converted to that type.</summary>
        /// <param name="type">The type of the null value.</param>
        /// <param name="nullValue">The string to be converted to the specified type.</param>
        public FieldNullValueAttribute(Type type, string nullValue) : this(Convert.ChangeType(nullValue, type, null))
        {
        }
    }
}