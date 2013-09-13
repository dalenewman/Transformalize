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
using Transformalize.Libs.FileHelpers.ErrorHandling;

namespace Transformalize.Libs.FileHelpers.Attributes
{
    /// <summary>Indicates that the engine must ignore commented lines while reading.</summary>
    /// <remarks>
    ///     See the <a href="attributes.html">Complete Attributes List</a> for more clear info and examples of each one.
    /// </remarks>
    /// <seealso href="attributes.html">Attributes List</seealso>
    /// <seealso href="quick_start.html">Quick Start Guide</seealso>
    /// <seealso href="examples.html">Examples of Use</seealso>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class IgnoreCommentedLinesAttribute : Attribute
    {
        internal bool mAnyPlace = true;
        internal string mCommentMarker;

        /// <summary>Indicates that the engine must ignore commented lines while reading. (The Comment Marker can appear in any place with spaces or tabs at his left)</summary>
        /// <param name="commentMarker">The comment marker used to ignore the lines</param>
        public IgnoreCommentedLinesAttribute(string commentMarker) : this(commentMarker, true)
        {
        }

        /// <summary>Indicates that the engine must ignore commented lines while reading.</summary>
        /// <param name="commentMarker">The comment marker used to ignore the lines</param>
        /// <param name="anyPlace">Indicates if the comment can have spaces or tabs at left (true by default)</param>
        public IgnoreCommentedLinesAttribute(string commentMarker, bool anyPlace)
        {
            if (commentMarker == null || commentMarker.Trim().Length == 0)
                throw new BadUsageException("The comment string parameter cant be null or empty.");

            mCommentMarker = commentMarker.Trim();
            mAnyPlace = anyPlace;
        }
    }
}