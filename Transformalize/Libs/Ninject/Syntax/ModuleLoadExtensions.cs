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

using System.Reflection;
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Modules;

#endregion

namespace Transformalize.Libs.Ninject.Syntax
{
    /// <summary>
    ///     Extension methods that enhance module loading.
    /// </summary>
    public static class ModuleLoadExtensions
    {
        /// <summary>
        ///     Creates a new instance of the module and loads it into the kernel.
        /// </summary>
        /// <typeparam name="TModule">The type of the module.</typeparam>
        /// <param name="kernel">The kernel.</param>
        public static void Load<TModule>(this IKernel kernel)
            where TModule : INinjectModule, new()
        {
            Ensure.ArgumentNotNull(kernel, "kernel");
            kernel.Load(new TModule());
        }

        /// <summary>
        ///     Loads the module(s) into the kernel.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="modules">The modules to load.</param>
        public static void Load(this IKernel kernel, params INinjectModule[] modules)
        {
            kernel.Load(modules);
        }

#if !NO_ASSEMBLY_SCANNING
        /// <summary>
        ///     Loads modules from the files that match the specified pattern(s).
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="filePatterns">The file patterns (i.e. "*.dll", "modules/*.rb") to match.</param>
        public static void Load(this IKernel kernel, params string[] filePatterns)
        {
            kernel.Load(filePatterns);
        }

        /// <summary>
        ///     Loads modules defined in the specified assemblies.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="assemblies">The assemblies to search.</param>
        public static void Load(this IKernel kernel, params Assembly[] assemblies)
        {
            kernel.Load(assemblies);
        }
#endif
    }
}