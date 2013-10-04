#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using Transformalize.Libs.FileHelpers.ErrorHandling;

namespace Transformalize.Libs.FileHelpers.Attributes
{
    /// <summary>Indicates a diferent delimiter for this field. </summary>
    /// <remarks>
    ///     See the <a href="attributes.html">Complete Attributes List</a> for more clear info and examples of each one.
    /// </remarks>
    /// <seealso href="attributes.html">Attributes List</seealso>
    /// <seealso href="quick_start.html">Quick Start Guide</seealso>
    /// <seealso href="examples.html">Examples of Use</seealso>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FieldDelimiterAttribute : FieldAttribute
    {
        internal string mSeparator;

        /// <summary>Indicates a diferent delimiter for this field. </summary>
        /// <param name="separator">The separator string used to split the fields of the record.</param>
        public FieldDelimiterAttribute(string separator)
        {
            if (separator == null || separator.Length == 0)
                throw new BadUsageException("The separator parameter of the FieldDelimited attribute can't be null or empty");
            else
                mSeparator = separator;
        }
    }
}