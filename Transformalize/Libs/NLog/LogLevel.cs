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
using System.Diagnostics.CodeAnalysis;
using Transformalize.Libs.NLog.Internal;

namespace Transformalize.Libs.NLog
{
    /// <summary>
    ///     Defines available log levels.
    /// </summary>
    public sealed class LogLevel : IComparable
    {
        /// <summary>
        ///     Trace log level.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable")] public static readonly LogLevel Trace = new LogLevel("Trace", 0);

        /// <summary>
        ///     Debug log level.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable")] public static readonly LogLevel Debug = new LogLevel("Debug", 1);

        /// <summary>
        ///     Info log level.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable")] public static readonly LogLevel Info = new LogLevel("Info", 2);

        /// <summary>
        ///     Warn log level.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable")] public static readonly LogLevel Warn = new LogLevel("Warn", 3);

        /// <summary>
        ///     Error log level.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable")] public static readonly LogLevel Error = new LogLevel("Error", 4);

        /// <summary>
        ///     Fatal log level.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable")] public static readonly LogLevel Fatal = new LogLevel("Fatal", 5);

        /// <summary>
        ///     Off log level.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable")] public static readonly LogLevel Off = new LogLevel("Off", 6);

        private readonly string name;
        private readonly int ordinal;

        // to be changed into public in the future.
        private LogLevel(string name, int ordinal)
        {
            this.name = name;
            this.ordinal = ordinal;
        }

        /// <summary>
        ///     Gets the name of the log level.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        internal static LogLevel MaxLevel
        {
            get { return Fatal; }
        }

        internal static LogLevel MinLevel
        {
            get { return Trace; }
        }

        /// <summary>
        ///     Gets the ordinal of the log level.
        /// </summary>
        internal int Ordinal
        {
            get { return ordinal; }
        }

        /// <summary>
        ///     Compares the level to the other <see cref="LogLevel" /> object.
        /// </summary>
        /// <param name="obj">
        ///     The object object.
        /// </param>
        /// <returns>
        ///     A value less than zero when this logger's <see cref="Ordinal" /> is
        ///     less than the other logger's ordinal, 0 when they are equal and
        ///     greater than zero when this ordinal is greater than the
        ///     other ordinal.
        /// </returns>
        public int CompareTo(object obj)
        {
            var level = (LogLevel) obj;

            return Ordinal - level.Ordinal;
        }

        /// <summary>
        ///     Compares two <see cref="LogLevel" /> objects
        ///     and returns a value indicating whether
        ///     the first one is equal to the second one.
        /// </summary>
        /// <param name="level1">The first level.</param>
        /// <param name="level2">The second level.</param>
        /// <returns>
        ///     The value of <c>level1.Ordinal == level2.Ordinal</c>.
        /// </returns>
        public static bool operator ==(LogLevel level1, LogLevel level2)
        {
            if (ReferenceEquals(level1, null))
            {
                return ReferenceEquals(level2, null);
            }

            if (ReferenceEquals(level2, null))
            {
                return false;
            }

            return level1.Ordinal == level2.Ordinal;
        }

        /// <summary>
        ///     Compares two <see cref="LogLevel" /> objects
        ///     and returns a value indicating whether
        ///     the first one is not equal to the second one.
        /// </summary>
        /// <param name="level1">The first level.</param>
        /// <param name="level2">The second level.</param>
        /// <returns>
        ///     The value of <c>level1.Ordinal != level2.Ordinal</c>.
        /// </returns>
        public static bool operator !=(LogLevel level1, LogLevel level2)
        {
            if (ReferenceEquals(level1, null))
            {
                return !ReferenceEquals(level2, null);
            }

            if (ReferenceEquals(level2, null))
            {
                return true;
            }

            return level1.Ordinal != level2.Ordinal;
        }

        /// <summary>
        ///     Compares two <see cref="LogLevel" /> objects
        ///     and returns a value indicating whether
        ///     the first one is greater than the second one.
        /// </summary>
        /// <param name="level1">The first level.</param>
        /// <param name="level2">The second level.</param>
        /// <returns>
        ///     The value of <c>level1.Ordinal &gt; level2.Ordinal</c>.
        /// </returns>
        public static bool operator >(LogLevel level1, LogLevel level2)
        {
            ParameterUtils.AssertNotNull(level1, "level1");
            ParameterUtils.AssertNotNull(level2, "level2");

            return level1.Ordinal > level2.Ordinal;
        }

        /// <summary>
        ///     Compares two <see cref="LogLevel" /> objects
        ///     and returns a value indicating whether
        ///     the first one is greater than or equal to the second one.
        /// </summary>
        /// <param name="level1">The first level.</param>
        /// <param name="level2">The second level.</param>
        /// <returns>
        ///     The value of <c>level1.Ordinal &gt;= level2.Ordinal</c>.
        /// </returns>
        public static bool operator >=(LogLevel level1, LogLevel level2)
        {
            ParameterUtils.AssertNotNull(level1, "level1");
            ParameterUtils.AssertNotNull(level2, "level2");

            return level1.Ordinal >= level2.Ordinal;
        }

        /// <summary>
        ///     Compares two <see cref="LogLevel" /> objects
        ///     and returns a value indicating whether
        ///     the first one is less than the second one.
        /// </summary>
        /// <param name="level1">The first level.</param>
        /// <param name="level2">The second level.</param>
        /// <returns>
        ///     The value of <c>level1.Ordinal &lt; level2.Ordinal</c>.
        /// </returns>
        public static bool operator <(LogLevel level1, LogLevel level2)
        {
            ParameterUtils.AssertNotNull(level1, "level1");
            ParameterUtils.AssertNotNull(level2, "level2");

            return level1.Ordinal < level2.Ordinal;
        }

        /// <summary>
        ///     Compares two <see cref="LogLevel" /> objects
        ///     and returns a value indicating whether
        ///     the first one is less than or equal to the second one.
        /// </summary>
        /// <param name="level1">The first level.</param>
        /// <param name="level2">The second level.</param>
        /// <returns>
        ///     The value of <c>level1.Ordinal &lt;= level2.Ordinal</c>.
        /// </returns>
        public static bool operator <=(LogLevel level1, LogLevel level2)
        {
            ParameterUtils.AssertNotNull(level1, "level1");
            ParameterUtils.AssertNotNull(level2, "level2");

            return level1.Ordinal <= level2.Ordinal;
        }

        /// <summary>
        ///     Gets the <see cref="LogLevel" /> that corresponds to the specified ordinal.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>
        ///     The <see cref="LogLevel" /> instance. For 0 it returns <see cref="LogLevel.Trace" />, 1 gives
        ///     <see
        ///         cref="LogLevel.Debug" />
        ///     and so on.
        /// </returns>
        public static LogLevel FromOrdinal(int ordinal)
        {
            switch (ordinal)
            {
                case 0:
                    return Trace;
                case 1:
                    return Debug;
                case 2:
                    return Info;
                case 3:
                    return Warn;
                case 4:
                    return Error;
                case 5:
                    return Fatal;
                case 6:
                    return Off;

                default:
                    throw new ArgumentException("Invalid ordinal.");
            }
        }

        /// <summary>
        ///     Returns the <see cref="T:Transformalize.Libs.NLog.LogLevel" /> that corresponds to the supplied
        ///     <see
        ///         langword="string" />
        ///     .
        /// </summary>
        /// <param name="levelName">The texual representation of the log level.</param>
        /// <returns>The enumeration value.</returns>
        public static LogLevel FromString(string levelName)
        {
            if (levelName == null)
            {
                throw new ArgumentNullException("levelName");
            }

            if (levelName.Equals("Trace", StringComparison.OrdinalIgnoreCase))
            {
                return Trace;
            }

            if (levelName.Equals("Debug", StringComparison.OrdinalIgnoreCase))
            {
                return Debug;
            }

            if (levelName.Equals("Info", StringComparison.OrdinalIgnoreCase))
            {
                return Info;
            }

            if (levelName.Equals("Warn", StringComparison.OrdinalIgnoreCase))
            {
                return Warn;
            }

            if (levelName.Equals("Error", StringComparison.OrdinalIgnoreCase))
            {
                return Error;
            }

            if (levelName.Equals("Fatal", StringComparison.OrdinalIgnoreCase))
            {
                return Fatal;
            }

            if (levelName.Equals("Off", StringComparison.OrdinalIgnoreCase))
            {
                return Off;
            }

            throw new ArgumentException("Unknown log level: " + levelName);
        }

        /// <summary>
        ///     Returns a string representation of the log level.
        /// </summary>
        /// <returns>Log level name.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return Ordinal;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">
        ///     The <see cref="System.Object" /> to compare with this instance.
        /// </param>
        /// <returns>
        ///     Value of <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        ///     The <paramref name="obj" /> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            var other = obj as LogLevel;
            if ((object) other == null)
            {
                return false;
            }

            return Ordinal == other.Ordinal;
        }
    }
}