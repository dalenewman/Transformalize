#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;

namespace Transformalize.Libs.NLog.Conditions
{
    /// <summary>
    ///     A bunch of utility methods (mostly predicates) which can be used in
    ///     condition expressions. Parially inspired by XPath 1.0.
    /// </summary>
    [ConditionMethods]
    public static class ConditionMethods
    {
        /// <summary>
        ///     Compares two values for equality.
        /// </summary>
        /// <param name="firstValue">The first value.</param>
        /// <param name="secondValue">The second value.</param>
        /// <returns>
        ///     <b>true</b> when two objects are equal, <b>false</b> otherwise.
        /// </returns>
        [ConditionMethod("equals")]
        public static bool Equals2(object firstValue, object secondValue)
        {
            return firstValue.Equals(secondValue);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the second string is a substring of the first one.
        /// </summary>
        /// <param name="haystack">The first string.</param>
        /// <param name="needle">The second string.</param>
        /// <returns>
        ///     <b>true</b> when the second string is a substring of the first string, <b>false</b> otherwise.
        /// </returns>
        [ConditionMethod("contains")]
        public static bool Contains(string haystack, string needle)
        {
            return haystack.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the second string is a prefix of the first one.
        /// </summary>
        /// <param name="haystack">The first string.</param>
        /// <param name="needle">The second string.</param>
        /// <returns>
        ///     <b>true</b> when the second string is a prefix of the first string, <b>false</b> otherwise.
        /// </returns>
        [ConditionMethod("starts-with")]
        public static bool StartsWith(string haystack, string needle)
        {
            return haystack.StartsWith(needle, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the second string is a suffix of the first one.
        /// </summary>
        /// <param name="haystack">The first string.</param>
        /// <param name="needle">The second string.</param>
        /// <returns>
        ///     <b>true</b> when the second string is a prefix of the first string, <b>false</b> otherwise.
        /// </returns>
        [ConditionMethod("ends-with")]
        public static bool EndsWith(string haystack, string needle)
        {
            return haystack.EndsWith(needle, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Returns the length of a string.
        /// </summary>
        /// <param name="text">A string whose lengths is to be evaluated.</param>
        /// <returns>The length of the string.</returns>
        [ConditionMethod("length")]
        public static int Length(string text)
        {
            return text.Length;
        }
    }
}