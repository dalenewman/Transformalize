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

#if !NO_ASSEMBLY_SCANNING

#region Using Directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Transformalize.Libs.Ninject.Components;
using Transformalize.Libs.Ninject.Infrastructure;

#endregion

namespace Transformalize.Libs.Ninject.Modules
{
    /// <summary>
    ///     Automatically finds and loads modules from assemblies.
    /// </summary>
    public class ModuleLoader : NinjectComponent, IModuleLoader
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ModuleLoader" /> class.
        /// </summary>
        /// <param name="kernel">The kernel into which modules will be loaded.</param>
        public ModuleLoader(IKernel kernel)
        {
            Ensure.ArgumentNotNull(kernel, "kernel");
            Kernel = kernel;
        }

        /// <summary>
        ///     Gets or sets the kernel into which modules will be loaded.
        /// </summary>
        public IKernel Kernel { get; private set; }

        /// <summary>
        ///     Loads any modules found in the files that match the specified patterns.
        /// </summary>
        /// <param name="patterns">The patterns to search.</param>
        public void LoadModules(IEnumerable<string> patterns)
        {
            var plugins = Kernel.Components.GetAll<IModuleLoaderPlugin>();

            var fileGroups = patterns
                .SelectMany(pattern => GetFilesMatchingPattern(pattern))
                .GroupBy(filename => Path.GetExtension(filename).ToLowerInvariant());

            foreach (var fileGroup in fileGroups)
            {
                var extension = fileGroup.Key;
                var plugin = plugins.Where(p => p.SupportedExtensions.Contains(extension)).FirstOrDefault();

                if (plugin != null)
                    plugin.LoadModules(fileGroup);
            }
        }

        private static IEnumerable<string> GetFilesMatchingPattern(string pattern)
        {
            return NormalizePaths(Path.GetDirectoryName(pattern))
                .SelectMany(path => Directory.GetFiles(path, Path.GetFileName(pattern)));
        }

        private static IEnumerable<string> NormalizePaths(string path)
        {
            return Path.IsPathRooted(path)
                       ? new[] {Path.GetFullPath(path)}
                       : GetBaseDirectories().Select(baseDirectory => Path.Combine(baseDirectory, path));
        }

        private static IEnumerable<string> GetBaseDirectories()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var searchPath = AppDomain.CurrentDomain.RelativeSearchPath;

            return String.IsNullOrEmpty(searchPath)
                       ? new[] {baseDirectory}
                       : searchPath.Split(new[] {Path.PathSeparator}, StringSplitOptions.RemoveEmptyEntries)
                                   .Select(path => Path.Combine(baseDirectory, path));
        }
    }
}

#endif
//!NO_ASSEMBLY_SCANNING