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

using Transformalize.Libs.Ninject.Modules;

#if !SILVERLIGHT && !NETCF

namespace Transformalize.Libs.Ninject
{
    /// <summary>
    ///     Registers the kernel into which the module is loaded on the GlobalKernelRegistry using the
    ///     type specified by TGlobalKernelRegistry.
    /// </summary>
    /// <typeparam name="TGlobalKernelRegistry">The type that is used to register the kernel.</typeparam>
    public abstract class GlobalKernelRegistrationModule<TGlobalKernelRegistry> : NinjectModule
        where TGlobalKernelRegistry : GlobalKernelRegistration
    {
        /// <summary>
        ///     Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            GlobalKernelRegistration.RegisterKernelForType(Kernel, typeof (TGlobalKernelRegistry));
        }

        /// <summary>
        ///     Unloads the module from the kernel.
        /// </summary>
        public override void Unload()
        {
            GlobalKernelRegistration.UnregisterKernelForType(Kernel, typeof (TGlobalKernelRegistry));
        }
    }
}

#endif