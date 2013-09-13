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

namespace Transformalize.Libs.Rhino.Etl
{
    /// <summary>
    ///     Helper class for guard statements, which allow prettier
    ///     code for guard clauses
    /// </summary>
    public class Guard
    {
        /// <summary>
        ///     Will throw a <see cref="InvalidOperationException" /> if the assertion
        ///     is true, with the specificied message.
        /// </summary>
        /// <param name="assertion">
        ///     if set to <c>true</c> [assertion].
        /// </param>
        /// <param name="message">The message.</param>
        /// <example>
        ///     Sample usage:
        ///     <code>
        /// Guard.Against(string.IsNullOrEmpty(name), "Name must have a value");
        /// </code>
        /// </example>
        public static void Against(bool assertion, string message)
        {
            if (assertion == false)
                return;
            throw new InvalidOperationException(message);
        }

        /// <summary>
        ///     Will throw exception of type <typeparamref name="TException" />
        ///     with the specified message if the assertion is true
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="assertion">
        ///     if set to <c>true</c> [assertion].
        /// </param>
        /// <param name="message">The message.</param>
        /// <example>
        ///     Sample usage:
        ///     <code>
        /// <![CDATA[
        /// Guard.Against<ArgumentException>(string.IsNullOrEmpty(name), "Name must have a value");
        /// ]]>
        /// </code>
        /// </example>
        public static void Against<TException>(bool assertion, string message) where TException : Exception
        {
            if (assertion == false)
                return;
            throw (TException) Activator.CreateInstance(typeof (TException), message);
        }
    }
}