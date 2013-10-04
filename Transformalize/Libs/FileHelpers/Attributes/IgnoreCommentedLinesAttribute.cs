#region License
// /*
// See license included in this library folder.
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