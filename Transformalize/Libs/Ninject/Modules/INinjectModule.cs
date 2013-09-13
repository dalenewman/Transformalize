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

#region Using Directives

using Transformalize.Libs.Ninject.Infrastructure;

#endregion

namespace Transformalize.Libs.Ninject.Modules
{
    /// <summary>
    ///     A pluggable unit that can be loaded into an <see cref="IKernel" />.
    /// </summary>
    public interface INinjectModule : IHaveKernel
    {
        /// <summary>
        ///     Gets the module's name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Called when the module is loaded into a kernel.
        /// </summary>
        /// <param name="kernel">The kernel that is loading the module.</param>
        void OnLoad(IKernel kernel);

        /// <summary>
        ///     Called when the module is unloaded from a kernel.
        /// </summary>
        /// <param name="kernel">The kernel that is unloading the module.</param>
        void OnUnload(IKernel kernel);

        /// <summary>
        ///     Called after loading the modules. A module can verify here if all other required modules are loaded.
        /// </summary>
        void OnVerifyRequiredModules();
    }
}