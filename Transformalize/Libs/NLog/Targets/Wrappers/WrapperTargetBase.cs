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
using Transformalize.Libs.NLog.Common;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.Targets.Wrappers
{
    /// <summary>
    ///     Base class for targets wrap other (single) targets.
    /// </summary>
    public abstract class WrapperTargetBase : Target
    {
        /// <summary>
        ///     Gets or sets the target that is wrapped by this target.
        /// </summary>
        /// <docgen category='General Options' order='11' />
        [RequiredParameter]
        public Target WrappedTarget { get; set; }

        /// <summary>
        ///     Returns the text representation of the object. Used for diagnostics.
        /// </summary>
        /// <returns>A string that describes the target.</returns>
        public override string ToString()
        {
            return base.ToString() + "(" + WrappedTarget + ")";
        }

        /// <summary>
        ///     Flush any pending log messages (in case of asynchronous targets).
        /// </summary>
        /// <param name="asyncContinuation">The asynchronous continuation.</param>
        protected override void FlushAsync(AsyncContinuation asyncContinuation)
        {
            WrappedTarget.Flush(asyncContinuation);
        }

        /// <summary>
        ///     Writes logging event to the log target. Must be overridden in inheriting
        ///     classes.
        /// </summary>
        /// <param name="logEvent">Logging event to be written out.</param>
        protected override sealed void Write(LogEventInfo logEvent)
        {
            throw new NotSupportedException("This target must not be invoked in a synchronous way.");
        }
    }
}