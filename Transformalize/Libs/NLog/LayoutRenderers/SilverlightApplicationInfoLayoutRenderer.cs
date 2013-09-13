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
    using System;
    using System.ComponentModel;
    using System.Text;
#if !DOCUMENTATION
    using System.Windows;
    using System.Windows.Browser;
#endif

    using NLog.Config;

    /// <summary>
    /// Information about Silverlight application.
    /// </summary>
    [LayoutRenderer("sl-appinfo")]
    public class SilverlightApplicationInfoLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SilverlightApplicationInfoLayoutRenderer"/> class.
        /// </summary>
        public SilverlightApplicationInfoLayoutRenderer()
        {
            this.Option = SilverlightApplicationInfoOption.XapUri;
        }

        /// <summary>
        /// Gets or sets specific information to display.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultParameter]
        [DefaultValue(SilverlightApplicationInfoOption.XapUri)]
        public SilverlightApplicationInfoOption Option { get; set; }

        /// <summary>
        /// Renders the specified environmental information and appends it to the specified <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
#if !DOCUMENTATION
            switch (this.Option)
            {
                case SilverlightApplicationInfoOption.XapUri:
                    builder.Append(Application.Current.Host.Source);
                    break;

#if !SILVERLIGHT2
                case SilverlightApplicationInfoOption.IsOutOfBrowser:
                    builder.Append(Application.Current.IsRunningOutOfBrowser ? "1" : "0");
                    break;

                case SilverlightApplicationInfoOption.InstallState:
                    builder.Append(Application.Current.InstallState);
                    break;
#if !SILVERLIGHT3
                case SilverlightApplicationInfoOption.HasElevatedPermissions:
                    builder.Append(Application.Current.HasElevatedPermissions ? "1" : "0");
                    break;
#endif

#endif
            }
#endif
        }
    }
}

#endif
}