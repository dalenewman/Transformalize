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

using System.IO;
using System.Text;
using Transformalize.Libs.NLog.Config;

#if !SILVERLIGHT

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     The directory where NLog.dll is located.
    /// </summary>
    [LayoutRenderer("nlogdir")]
    [AppDomainFixedOutput]
    [ThreadAgnostic]
    public class NLogDirLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Initializes static members of the NLogDirLayoutRenderer class.
        /// </summary>
        static NLogDirLayoutRenderer()
        {
#if !NET_CF
            NLogDir = Path.GetDirectoryName(typeof (LogManager).Assembly.Location);
#else
            NLogDir = Path.GetDirectoryName(typeof(LogManager).Assembly.GetName().CodeBase);
#endif
        }

        /// <summary>
        ///     Gets or sets the name of the file to be Path.Combine()'d with the directory name.
        /// </summary>
        /// <docgen category='Advanced Options' order='10' />
        public string File { get; set; }

        /// <summary>
        ///     Gets or sets the name of the directory to be Path.Combine()'d with the directory name.
        /// </summary>
        /// <docgen category='Advanced Options' order='10' />
        public string Dir { get; set; }

        private static string NLogDir { get; set; }

        /// <summary>
        ///     Renders the directory where NLog is located and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var baseDir = NLogDir;

            if (File != null)
            {
                builder.Append(Path.Combine(baseDir, File));
            }
            else if (Dir != null)
            {
                builder.Append(Path.Combine(baseDir, Dir));
            }
            else
            {
                builder.Append(baseDir);
            }
        }
    }
}

#endif