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

using System.Collections.Generic;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Filters;
using Transformalize.Libs.NLog.Targets;

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Represents target with a chain of filters which determine
    ///     whether logging should happen.
    /// </summary>
    [NLogConfigurationItem]
    internal class TargetWithFilterChain
    {
        private StackTraceUsage stackTraceUsage = StackTraceUsage.None;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TargetWithFilterChain" /> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="filterChain">The filter chain.</param>
        public TargetWithFilterChain(Target target, IList<Filter> filterChain)
        {
            Target = target;
            FilterChain = filterChain;
            stackTraceUsage = StackTraceUsage.None;
        }

        /// <summary>
        ///     Gets the target.
        /// </summary>
        /// <value>The target.</value>
        public Target Target { get; private set; }

        /// <summary>
        ///     Gets the filter chain.
        /// </summary>
        /// <value>The filter chain.</value>
        public IList<Filter> FilterChain { get; private set; }

        /// <summary>
        ///     Gets or sets the next <see cref="TargetWithFilterChain" /> item in the chain.
        /// </summary>
        /// <value>The next item in the chain.</value>
        public TargetWithFilterChain NextInChain { get; set; }

        /// <summary>
        ///     Gets the stack trace usage.
        /// </summary>
        /// <returns>
        ///     A <see cref="StackTraceUsage" /> value that determines stack trace handling.
        /// </returns>
        public StackTraceUsage GetStackTraceUsage()
        {
            return stackTraceUsage;
        }

        internal void PrecalculateStackTraceUsage()
        {
            stackTraceUsage = StackTraceUsage.None;

            // find all objects which may need stack trace
            // and determine maximum
            foreach (var item in ObjectGraphScanner.FindReachableObjects<IUsesStackTrace>(this))
            {
                var stu = item.StackTraceUsage;

                if (stu > stackTraceUsage)
                {
                    stackTraceUsage = stu;

                    if (stackTraceUsage >= StackTraceUsage.Max)
                    {
                        break;
                    }
                }
            }
        }
    }
}