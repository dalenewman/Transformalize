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