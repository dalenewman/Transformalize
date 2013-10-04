#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using Transformalize.Libs.Ninject.Infrastructure;

#endregion

namespace Transformalize.Libs.Ninject.Modules
{
    /// <summary>
    ///     A pluggable unit that can be loaded into an <see cref="IKernel" />.
    /// </summary>
    public interface INinjectModule : IHaveKernel
    {
        /// <summary>
        ///     Gets the module's name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Called when the module is loaded into a kernel.
        /// </summary>
        /// <param name="kernel">The kernel that is loading the module.</param>
        void OnLoad(IKernel kernel);

        /// <summary>
        ///     Called when the module is unloaded from a kernel.
        /// </summary>
        /// <param name="kernel">The kernel that is unloading the module.</param>
        void OnUnload(IKernel kernel);

        /// <summary>
        ///     Called after loading the modules. A module can verify here if all other required modules are loaded.
        /// </summary>
        void OnVerifyRequiredModules();
    }
}