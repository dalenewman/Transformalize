#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Supported operating systems.
    /// </summary>
    /// <remarks>
    ///     If you add anything here, make sure to add the appropriate detection
    ///     code to <see cref="PlatformDetector" />
    /// </remarks>
    internal enum RuntimeOS
    {
        /// <summary>
        ///     Any operating system.
        /// </summary>
        Any,

        /// <summary>
        ///     Unix/Linux operating systems.
        /// </summary>
        Unix,

        /// <summary>
        ///     Windows CE.
        /// </summary>
        WindowsCE,

        /// <summary>
        ///     Desktop versions of Windows (95,98,ME).
        /// </summary>
        Windows,

        /// <summary>
        ///     Windows NT, 2000, 2003 and future versions based on NT technology.
        /// </summary>
        WindowsNT,

        /// <summary>
        ///     Unknown operating system.
        /// </summary>
        Unknown,
    }
}