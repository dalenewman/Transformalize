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

namespace Transformalize.Libs.NLog.LayoutRenderers
{
#if (SILVERLIGHT || DOCUMENTATION) && !WINDOWS_PHONE

namespace NLog.LayoutRenderers
{
    using System.Text;
#if !DOCUMENTATION
    using System.Windows.Browser;
#endif

    /// <summary>
    /// URI of the HTML page which hosts the current Silverlight application.
    /// </summary>
    [LayoutRenderer("document-uri")]
    public class DocumentUriLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        /// Renders the specified environmental information and appends it to the specified <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
#if !DOCUMENTATION
            builder.Append(HtmlPage.Document.DocumentUri.ToString());
#endif
        }
    }
}

#endif
}