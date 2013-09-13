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
using Transformalize.Libs.NLog.Common;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Internal;

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     The base class for all targets which call methods (local or remote).
    ///     Manages parameters and type coercion.
    /// </summary>
    public abstract class MethodCallTargetBase : Target
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MethodCallTargetBase" /> class.
        /// </summary>
        protected MethodCallTargetBase()
        {
            Parameters = new List<MethodCallParameter>();
        }

        /// <summary>
        ///     Gets the array of parameters to be passed.
        /// </summary>
        /// <docgen category='Parameter Options' order='10' />
        [ArrayParameter(typeof (MethodCallParameter), "parameter")]
        public IList<MethodCallParameter> Parameters { get; private set; }

        /// <summary>
        ///     Prepares an array of parameters to be passed based on the logging event and calls DoInvoke().
        /// </summary>
        /// <param name="logEvent">
        ///     The logging event.
        /// </param>
        protected override void Write(AsyncLogEventInfo logEvent)
        {
            var parameters = new object[Parameters.Count];
            var i = 0;

            foreach (var mcp in Parameters)
            {
                parameters[i++] = mcp.GetValue(logEvent.LogEvent);
            }

            DoInvoke(parameters, logEvent.Continuation);
        }

        /// <summary>
        ///     Calls the target method. Must be implemented in concrete classes.
        /// </summary>
        /// <param name="parameters">Method call parameters.</param>
        /// <param name="continuation">The continuation.</param>
        protected virtual void DoInvoke(object[] parameters, AsyncContinuation continuation)
        {
            try
            {
                DoInvoke(parameters);
                continuation(null);
            }
            catch (Exception ex)
            {
                if (ex.MustBeRethrown())
                {
                    throw;
                }

                continuation(ex);
            }
        }

        /// <summary>
        ///     Calls the target method. Must be implemented in concrete classes.
        /// </summary>
        /// <param name="parameters">Method call parameters.</param>
        protected abstract void DoInvoke(object[] parameters);
    }
}