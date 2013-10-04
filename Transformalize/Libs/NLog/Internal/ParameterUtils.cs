#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Parameter validation utilities.
    /// </summary>
    internal static class ParameterUtils
    {
        /// <summary>
        ///     Asserts that the value is not null and throws <see cref="ArgumentNullException" /> otherwise.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        public static void AssertNotNull(object value, string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }
    }
}