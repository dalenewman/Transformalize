#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using Transformalize.Libs.Ninject.Components;

#if !NO_ASSEMBLY_SCANNING

namespace Transformalize.Libs.Ninject.Modules
{
    /// <summary>
    ///     Retrieves assembly names from file names using a temporary app domain.
    /// </summary>
    public interface IAssemblyNameRetriever : INinjectComponent
    {
        /// <summary>
        ///     Gets all assembly names of the assemblies in the given files that match the filter.
        /// </summary>
        /// <param name="filenames">The filenames.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>All assembly names of the assemblies in the given files that match the filter.</returns>
        IEnumerable<AssemblyName> GetAssemblyNames(IEnumerable<string> filenames, Predicate<Assembly> filter);
    }
}

#endif