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
using System.Linq;
using System.Reflection;
using Transformalize.Libs.Ninject.Modules;

#endregion

namespace Transformalize.Libs.Ninject.Infrastructure.Language
{
    internal static class ExtensionsForAssembly
    {
        public static bool HasNinjectModules(this Assembly assembly)
        {
            return assembly.GetExportedTypes().Any(IsLoadableModule);
        }

        public static IEnumerable<INinjectModule> GetNinjectModules(this Assembly assembly)
        {
            return assembly.GetExportedTypes()
                           .Where(IsLoadableModule)
                           .Select(type => Activator.CreateInstance(type) as INinjectModule);
        }

        private static bool IsLoadableModule(Type type)
        {
            return typeof (INinjectModule).IsAssignableFrom(type)
                   && !type.IsAbstract
                   && !type.IsInterface
                   && type.GetConstructor(Type.EmptyTypes) != null;
        }
    }
}

#endif
//!NO_ASSEMBLY_SCANNING