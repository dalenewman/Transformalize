#region License
// /*
// See license included in this library folder.
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