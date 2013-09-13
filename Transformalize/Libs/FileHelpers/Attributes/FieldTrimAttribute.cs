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
using Transformalize.Libs.FileHelpers.Enums;

namespace Transformalize.Libs.FileHelpers.Attributes
{
    /// <summary>
    ///     Indicates the <see cref="TrimMode" /> used after read to truncate the field.
    /// </summary>
    /// <remarks>
    ///     See the <a href="attributes.html">Complete Attributes List</a> for more clear info and examples of each one.
    /// </remarks>
    /// <seealso href="attributes.html">Attributes List</seealso>
    /// <seealso href="quick_start.html">Quick Start Guide</seealso>
    /// <seealso href="examples.html">Examples of Use</seealso>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FieldTrimAttribute : Attribute
    {
        private static readonly char[] WhitespaceChars = new[]
                                                             {
                                                                 '\t', '\n', '\v', '\f', '\r', ' ', '\x00a0', '\u2000', '\u2001', '\u2002', '\u2003', '\u2004', '\u2005', '\u2006', '\u2007', '\u2008',
                                                                 '\u2009', '\u200a', '\u200b', '\u3000', '\ufeff'
                                                             };

        #region "  Constructors  "

        /// <summary>
        ///     Indicates the <see cref="TrimMode" /> used after read to truncate the field. By default trims the blank spaces and tabs.
        /// </summary>
        /// <param name="mode">
        ///     The <see cref="TrimMode" /> used after read.
        /// </param>
        public FieldTrimAttribute(TrimMode mode)
            : this(mode, WhitespaceChars)
        {
        }

        /// <summary>
        ///     Indicates the <see cref="TrimMode" /> used after read to truncate the field.
        /// </summary>
        /// <param name="mode">
        ///     The <see cref="TrimMode" /> used after read.
        /// </param>
        /// <param name="chars">A list of chars used to trim.</param>
        public FieldTrimAttribute(TrimMode mode, params char[] chars)
        {
            TrimMode = mode;
            Array.Sort(chars);
            TrimChars = chars;
        }

        /// <summary>
        ///     Indicates the <see cref="TrimMode" /> used after read to truncate the field.
        /// </summary>
        /// <param name="mode">
        ///     The <see cref="TrimMode" /> used after read.
        /// </param>
        /// <param name="trimChars">A string of chars used to trim.</param>
        public FieldTrimAttribute(TrimMode mode, string trimChars)
            : this(mode, trimChars.ToCharArray())
        {
        }

        #endregion

        internal Char[] TrimChars;
        internal TrimMode TrimMode;
    }
}