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

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Logger configuration.
    /// </summary>
    internal class LoggerConfiguration
    {
        private readonly TargetWithFilterChain[] targetsByLevel;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoggerConfiguration" /> class.
        /// </summary>
        /// <param name="targetsByLevel">The targets by level.</param>
        public LoggerConfiguration(TargetWithFilterChain[] targetsByLevel)
        {
            this.targetsByLevel = targetsByLevel;
        }

        /// <summary>
        ///     Gets targets for the specified level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>Chain of targets with attached filters.</returns>
        public TargetWithFilterChain GetTargetsForLevel(LogLevel level)
        {
            return targetsByLevel[level.Ordinal];
        }

        /// <summary>
        ///     Determines whether the specified level is enabled.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>
        ///     A value of <c>true</c> if the specified level is enabled; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEnabled(LogLevel level)
        {
            return targetsByLevel[level.Ordinal] != null;
        }
    }
}