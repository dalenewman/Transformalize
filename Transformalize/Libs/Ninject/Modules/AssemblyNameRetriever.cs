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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Transformalize.Libs.Ninject.Components;

#if !NO_ASSEMBLY_SCANNING

namespace Transformalize.Libs.Ninject.Modules
{
    /// <summary>
    ///     Retrieves assembly names from file names using a temporary app domain.
    /// </summary>
    public class AssemblyNameRetriever : NinjectComponent, IAssemblyNameRetriever
    {
        /// <summary>
        ///     Gets all assembly names of the assemblies in the given files that match the filter.
        /// </summary>
        /// <param name="filenames">The filenames.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>All assembly names of the assemblies in the given files that match the filter.</returns>
        public IEnumerable<AssemblyName> GetAssemblyNames(IEnumerable<string> filenames, Predicate<Assembly> filter)
        {
            var assemblyCheckerType = typeof (AssemblyChecker);
            var temporaryDomain = CreateTemporaryAppDomain();
            try
            {
                var checker = (AssemblyChecker) temporaryDomain.CreateInstanceAndUnwrap(
                    assemblyCheckerType.Assembly.FullName,
                    assemblyCheckerType.FullName ?? string.Empty);

                return checker.GetAssemblyNames(filenames.ToArray(), filter);
            }
            finally
            {
                AppDomain.Unload(temporaryDomain);
            }
        }

        /// <summary>
        ///     Creates a temporary app domain.
        /// </summary>
        /// <returns>The created app domain.</returns>
        private static AppDomain CreateTemporaryAppDomain()
        {
            return AppDomain.CreateDomain(
                "NinjectModuleLoader",
                AppDomain.CurrentDomain.Evidence,
                AppDomain.CurrentDomain.SetupInformation);
        }

        /// <summary>
        ///     This class is loaded into the temporary appdomain to load and check if the assemblies match the filter.
        /// </summary>
        private class AssemblyChecker : MarshalByRefObject
        {
            /// <summary>
            ///     Gets the assembly names of the assemblies matching the filter.
            /// </summary>
            /// <param name="filenames">The filenames.</param>
            /// <param name="filter">The filter.</param>
            /// <returns>All assembly names of the assemblies matching the filter.</returns>
            public IEnumerable<AssemblyName> GetAssemblyNames(IEnumerable<string> filenames, Predicate<Assembly> filter)
            {
                var result = new List<AssemblyName>();
                foreach (var filename in filenames)
                {
                    Assembly assembly;
                    if (File.Exists(filename))
                    {
                        try
                        {
                            assembly = Assembly.LoadFrom(filename);
                        }
                        catch (BadImageFormatException)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        try
                        {
                            assembly = Assembly.Load(filename);
                        }
                        catch (FileNotFoundException)
                        {
                            continue;
                        }
                    }

                    if (filter(assembly))
                    {
                        result.Add(assembly.GetName(false));
                    }
                }

                return result;
            }
        }
    }
}

#endif