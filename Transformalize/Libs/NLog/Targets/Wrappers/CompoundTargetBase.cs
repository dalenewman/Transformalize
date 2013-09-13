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
using System.Collections.Generic;
using System.Text;
using Transformalize.Libs.NLog.Common;

namespace Transformalize.Libs.NLog.Targets.Wrappers
{
    /// <summary>
    ///     A base class for targets which wrap other (multiple) targets
    ///     and provide various forms of target routing.
    /// </summary>
    public abstract class CompoundTargetBase : Target
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CompoundTargetBase" /> class.
        /// </summary>
        /// <param name="targets">The targets.</param>
        protected CompoundTargetBase(params Target[] targets)
        {
            Targets = new List<Target>(targets);
        }

        /// <summary>
        ///     Gets the collection of targets managed by this compound target.
        /// </summary>
        public IList<Target> Targets { get; private set; }

        /// <summary>
        ///     Returns the text representation of the object. Used for diagnostics.
        /// </summary>
        /// <returns>A string that describes the target.</returns>
        public override string ToString()
        {
            var separator = string.Empty;
            var sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append("(");

            foreach (var t in Targets)
            {
                sb.Append(separator);
                sb.Append(t);
                separator = ", ";
            }

            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        ///     Writes logging event to the log target.
        /// </summary>
        /// <param name="logEvent">Logging event to be written out.</param>
        protected override void Write(LogEventInfo logEvent)
        {
            throw new NotSupportedException("This target must not be invoked in a synchronous way.");
        }

        /// <summary>
        ///     Flush any pending log messages for all wrapped targets.
        /// </summary>
        /// <param name="asyncContinuation">The asynchronous continuation.</param>
        protected override void FlushAsync(AsyncContinuation asyncContinuation)
        {
            AsyncHelpers.ForEachItemInParallel(Targets, asyncContinuation, (t, c) => t.Flush(c));
        }
    }
}