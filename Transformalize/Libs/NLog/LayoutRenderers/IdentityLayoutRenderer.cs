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
using System.Threading;

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     Thread identity information (name and authentication information).
    /// </summary>
    [LayoutRenderer("identity")]
    public class IdentityLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="IdentityLayoutRenderer" /> class.
        /// </summary>
        public IdentityLayoutRenderer()
        {
            Name = true;
            AuthType = true;
            IsAuthenticated = true;
            Separator = ":";
        }

        /// <summary>
        ///     Gets or sets the separator to be used when concatenating
        ///     parts of identity information.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(":")]
        public string Separator { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to render Thread.CurrentPrincipal.Identity.Name.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(true)]
        public bool Name { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to render Thread.CurrentPrincipal.Identity.AuthenticationType.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(true)]
        public bool AuthType { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to render Thread.CurrentPrincipal.Identity.IsAuthenticated.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(true)]
        public bool IsAuthenticated { get; set; }

        /// <summary>
        ///     Renders the specified identity information and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var principal = Thread.CurrentPrincipal;
            if (principal != null)
            {
                var identity = principal.Identity;
                if (identity != null)
                {
                    var separator = string.Empty;

                    if (IsAuthenticated)
                    {
                        builder.Append(separator);
                        separator = Separator;

                        if (identity.IsAuthenticated)
                        {
                            builder.Append("auth");
                        }
                        else
                        {
                            builder.Append("notauth");
                        }
                    }

                    if (AuthType)
                    {
                        builder.Append(separator);
                        separator = Separator;
                        builder.Append(identity.AuthenticationType);
                    }

                    if (Name)
                    {
                        builder.Append(separator);
                        builder.Append(identity.Name);
                    }
                }
            }
        }
    }
}

#endif