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
using System.Text;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Internal;

#if !SILVERLIGHT

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     The name of the current process.
    /// </summary>
    [LayoutRenderer("processname")]
    [AppDomainFixedOutput]
    [ThreadAgnostic]
    public class ProcessNameLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Gets or sets a value indicating whether to write the full path to the process executable.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(false)]
        public bool FullName { get; set; }

        /// <summary>
        ///     Renders the current process name (optionally with a full path).
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (FullName)
            {
                builder.Append(ThreadIDHelper.Instance.CurrentProcessName);
            }
            else
            {
                builder.Append(ThreadIDHelper.Instance.CurrentProcessBaseName);
            }
        }
    }
}

#endif