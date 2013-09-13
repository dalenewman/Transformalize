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

using System.Text;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Internal;

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     The formatted log message.
    /// </summary>
    [LayoutRenderer("message")]
    [ThreadAgnostic]
    public class MessageLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageLayoutRenderer" /> class.
        /// </summary>
        public MessageLayoutRenderer()
        {
            ExceptionSeparator = EnvironmentHelper.NewLine;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to log exception along with message.
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        public bool WithException { get; set; }

        /// <summary>
        ///     Gets or sets the string that separates message from the exception.
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        public string ExceptionSeparator { get; set; }

        /// <summary>
        ///     Renders the log message including any positional parameters and appends it to the specified
        ///     <see
        ///         cref="StringBuilder" />
        ///     .
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(logEvent.FormattedMessage);
            if (WithException && logEvent.Exception != null)
            {
                builder.Append(ExceptionSeparator);
                builder.Append(logEvent.Exception);
            }
        }
    }
}