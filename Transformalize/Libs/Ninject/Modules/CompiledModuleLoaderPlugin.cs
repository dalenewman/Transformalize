#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Transformalize.Libs.Ninject.Components;
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Infrastructure.Language;

#if !NO_ASSEMBLY_SCANNING

namespace Transformalize.Libs.Ninject.Modules
{
    /// <summary>
    ///     Loads modules from compiled assemblies.
    /// </summary>
    public class CompiledModuleLoaderPlugin : NinjectComponent, IModuleLoaderPlugin
    {
        /// <summary>
        ///     The file extensions that are supported.
        /// </summary>
        private static readonly string[] Extensions = new[] {".dll"};

        /// <summary>
        ///     The assembly name retriever.
        /// </summary>
        private readonly IAssemblyNameRetriever assemblyNameRetriever;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompiledModuleLoaderPlugin" /> class.
        /// </summary>
        /// <param name="kernel">The kernel into which modules will be loaded.</param>
        /// <param name="assemblyNameRetriever">The assembly name retriever.</param>
        public CompiledModuleLoaderPlugin(IKernel kernel, IAssemblyNameRetriever assemblyNameRetriever)
        {
            Ensure.ArgumentNotNull(kernel, "kernel");
            Kernel = kernel;
            this.assemblyNameRetriever = assemblyNameRetriever;
        }

        /// <summary>
        ///     Gets the kernel into which modules will be loaded.
        /// </summary>
        public IKernel Kernel { get; private set; }

        /// <summary>
        ///     Gets the file extensions that the plugin understands how to load.
        /// </summary>
        public IEnumerable<string> SupportedExtensions
        {
            get { return Extensions; }
        }

        /// <summary>
        ///     Loads modules from the specified files.
        /// </summary>
        /// <param name="filenames">The names of the files to load modules from.</param>
        public void LoadModules(IEnumerable<string> filenames)
        {
            var assembliesWithModules = assemblyNameRetriever.GetAssemblyNames(filenames, asm => asm.HasNinjectModules());
            Kernel.Load(assembliesWithModules.Select(asm => Assembly.Load(asm)));
        }
    }
}

#endif