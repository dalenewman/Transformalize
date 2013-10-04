#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using Transformalize.Libs.FileHelpers.ErrorHandling;

namespace Transformalize.Libs.FileHelpers.Attributes
{
    /// <summary>Indicates the length of a FixedLength field.</summary>
    /// <remarks>
    ///     See the <a href="attributes.html">Complete Attributes List</a> for more clear info and examples of each one.
    /// </remarks>
    /// <seealso href="attributes.html">Attributes List</seealso>
    /// <seealso href="quick_start.html">Quick Start Guide</seealso>
    /// <seealso href="examples.html">Examples of Use</seealso>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FieldFixedLengthAttribute : FieldAttribute
    {
        internal int Length;

        /// <summary>Indicates the length of a FixedLength field.</summary>
        /// <param name="length">The length of the field.</param>
        public FieldFixedLengthAttribute(int length)
        {
            if (length > 0)
                Length = length;
            else
                throw new BadUsageException("The length parameter must be > 0");
        }
    }
}