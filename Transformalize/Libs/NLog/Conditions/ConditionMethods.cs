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