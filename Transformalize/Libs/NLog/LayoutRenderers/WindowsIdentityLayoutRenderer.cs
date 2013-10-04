#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.ComponentModel;
using System.Security.Principal;
using System.Text;

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     Thread Windows identity information (username).
    /// </summary>
    [LayoutRenderer("windows-identity")]
    public class WindowsIdentityLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="WindowsIdentityLayoutRenderer" /> class.
        /// </summary>
        public WindowsIdentityLayoutRenderer()
        {
            UserName = true;
            Domain = true;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether domain name should be included.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(true)]
        public bool Domain { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether username should be included.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(true)]
        public bool UserName { get; set; }

        /// <summary>
        ///     Renders the current thread windows identity information and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var currentIdentity = WindowsIdentity.GetCurrent();
            if (currentIdentity != null)
            {
                var output = string.Empty;

                if (UserName)
                {
                    if (Domain)
                    {
                        // username && domain
                        output = currentIdentity.Name;
                    }
                    else
                    {
                        // user name but no domain
                        var pos = currentIdentity.Name.LastIndexOf('\\');
                        if (pos >= 0)
                        {
                            output = currentIdentity.Name.Substring(pos + 1);
                        }
                        else
                        {
                            output = currentIdentity.Name;
                        }
                    }
                }
                else
                {
                    // no username
                    if (!Domain)
                    {
                        // nothing to output
                        return;
                    }

                    var pos = currentIdentity.Name.IndexOf('\\');
                    if (pos >= 0)
                    {
                        output = currentIdentity.Name.Substring(0, pos);
                    }
                    else
                    {
                        output = currentIdentity.Name;
                    }
                }

                builder.Append(output);
            }
        }
    }
}

#endif