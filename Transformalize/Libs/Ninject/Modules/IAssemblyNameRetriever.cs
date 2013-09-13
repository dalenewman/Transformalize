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