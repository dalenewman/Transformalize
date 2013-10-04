#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NLog.Config
{
    /// <summary>
    ///     Implemented by objects which support installation and uninstallation.
    /// </summary>
    public interface IInstallable
    {
        /// <summary>
        ///     Performs installation which requires administrative permissions.
        /// </summary>
        /// <param name="installationContext">The installation context.</param>
        void Install(InstallationContext installationContext);

        /// <summary>
        ///     Performs uninstallation which requires administrative permissions.
        /// </summary>
        /// <param name="installationContext">The installation context.</param>
        void Uninstall(InstallationContext installationContext);

        /// <summary>
        ///     Determines whether the item is installed.
        /// </summary>
        /// <param name="installationContext">The installation context.</param>
        /// <returns>
        ///     Value indicating whether the item is installed or null if it is not possible to determine.
        /// </returns>
        bool? IsInstalled(InstallationContext installationContext);
    }
}