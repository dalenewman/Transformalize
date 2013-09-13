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

using System.ComponentModel;

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Discards log messages. Used mainly for debugging and benchmarking.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/Null_target">Documentation on NLog Wiki</seealso>
    /// <example>
    ///     <p>
    ///         To set up the target in the <a href="config.html">configuration file</a>,
    ///         use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/Null/NLog.config" />
    ///     <p>
    ///         This assumes just one target and a single rule. More configuration
    ///         options are described <a href="config.html">here</a>.
    ///     </p>
    ///     <p>
    ///         To set up the log target programmatically use code like this:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/Null/Simple/Example.cs" />
    /// </example>
    [Target("Null")]
    public sealed class NullTarget : TargetWithLayout
    {
        /// <summary>
        ///     Gets or sets a value indicating whether to perform layout calculation.
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        [DefaultValue(false)]
        public bool FormatMessage { get; set; }

        /// <summary>
        ///     Does nothing. Optionally it calculates the layout text but
        ///     discards the results.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        protected override void Write(LogEventInfo logEvent)
        {
            if (FormatMessage)
            {
                Layout.Render(logEvent);
            }
        }
    }
}