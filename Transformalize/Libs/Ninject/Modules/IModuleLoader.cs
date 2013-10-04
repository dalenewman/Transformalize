#region License
// /*
// See license included in this library folder.
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