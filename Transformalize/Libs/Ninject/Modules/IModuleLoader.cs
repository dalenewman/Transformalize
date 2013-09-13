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

#if !SILVERLIGHT

#region Using Directives

using System.Collections.Generic;
using Transformalize.Libs.Ninject.Components;

#endregion

namespace Transformalize.Libs.Ninject.Modules
{
    /// <summary>
    ///     Finds modules defined in external files.
    /// </summary>
    public interface IModuleLoader : INinjectComponent
    {
        /// <summary>
        ///     Loads any modules found in the files that match the specified patterns.
        /// </summary>
        /// <param name="patterns">The patterns to search.</param>
        void LoadModules(IEnumerable<string> patterns);
    }
}

#endif