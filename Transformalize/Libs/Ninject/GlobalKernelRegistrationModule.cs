#region License
// /*
// See license included in this library folder.
// */
#endregion

using Transformalize.Libs.Ninject.Modules;

#if !SILVERLIGHT && !NETCF

namespace Transformalize.Libs.Ninject
{
    /// <summary>
    ///     Registers the kernel into which the module is loaded on the GlobalKernelRegistry using the
    ///     type specified by TGlobalKernelRegistry.
    /// </summary>
    /// <typeparam name="TGlobalKernelRegistry">The type that is used to register the kernel.</typeparam>
    public abstract class GlobalKernelRegistrationModule<TGlobalKernelRegistry> : NinjectModule
        where TGlobalKernelRegistry : GlobalKernelRegistration
    {
        /// <summary>
        ///     Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            GlobalKernelRegistration.RegisterKernelForType(Kernel, typeof (TGlobalKernelRegistry));
        }

        /// <summary>
        ///     Unloads the module from the kernel.
        /// </summary>
        public override void Unload()
        {
            GlobalKernelRegistration.UnregisterKernelForType(Kernel, typeof (TGlobalKernelRegistry));
        }
    }
}

#endif