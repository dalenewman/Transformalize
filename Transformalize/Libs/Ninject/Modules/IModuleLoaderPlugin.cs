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
    ///     Loads modules at runtime by searching external files.
    /// </summary>
    public interface IModuleLoaderPlugin : INinjectComponent
    {
        /// <summary>
        ///     Gets the file extensions that the plugin understands how to load.
        /// </summary>
        IEnumerable<string> SupportedExtensions { get; }

        /// <summary>
        ///     Loads modules from the specified files.
        /// </summary>
        /// <param name="filenames">The names of the files to load modules from.</param>
        void LoadModules(IEnumerable<string> filenames);
    }
}

#endif