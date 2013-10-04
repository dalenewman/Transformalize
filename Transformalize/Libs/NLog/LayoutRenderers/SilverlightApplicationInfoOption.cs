#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NLog.LayoutRenderers
{
#if (SILVERLIGHT || DOCUMENTATION) && !WINDOWS_PHONE

namespace NLog.LayoutRenderers
{
    /// <summary>
    /// Specifies application information to display in ${sl-appinfo} renderer.
    /// </summary>
    public enum SilverlightApplicationInfoOption
    {
        /// <summary>
        /// URI of the current application XAP file.
        /// </summary>
        XapUri,

#if !SILVERLIGHT2
        /// <summary>
        /// Whether application is running out-of-browser.
        /// </summary>
        IsOutOfBrowser,

        /// <summary>
        /// Installed state of an application.
        /// </summary>
        InstallState,

#if !SILVERLIGHT3
        /// <summary>
        /// Whether application is running with elevated permissions.
        /// </summary>
        HasElevatedPermissions,
#endif

#endif
    }
}

#endif
}