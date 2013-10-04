#region License
// /*
// See license included in this library folder.
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