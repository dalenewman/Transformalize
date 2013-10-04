#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Infrastructure.Language;
using Transformalize.Libs.Ninject.Planning.Bindings;
using Transformalize.Libs.Ninject.Syntax;

namespace Transformalize.Libs.Ninject.Modules
{
    /// <summary>
    ///     A loadable unit that defines bindings for your application.
    /// </summary>
    public abstract class NinjectModule : BindingRoot, INinjectModule
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NinjectModule" /> class.
        /// </summary>
        protected NinjectModule()
        {
            Bindings = new List<IBinding>();
        }

        /// <summary>
        ///     Gets the bindings that were registered by the module.
        /// </summary>
        public ICollection<IBinding> Bindings { get; private set; }

        /// <summary>
        ///     Gets the kernel.
        /// </summary>
        /// <value>The kernel.</value>
        protected override IKernel KernelInstance
        {
            get { return Kernel; }
        }

        /// <summary>
        ///     Gets the kernel that the module is loaded into.
        /// </summary>
        public IKernel Kernel { get; private set; }

        /// <summary>
        ///     Gets the module's name. Only a single module with a given name can be loaded at one time.
        /// </summary>
        public virtual string Name
        {
            get { return GetType().FullName; }
        }

        /// <summary>
        ///     Called when the module is loaded into a kernel.
        /// </summary>
        /// <param name="kernel">The kernel that is loading the module.</param>
        public void OnLoad(IKernel kernel)
        {
            Ensure.ArgumentNotNull(kernel, "kernel");
            Kernel = kernel;
            Load();
        }

        /// <summary>
        ///     Called when the module is unloaded from a kernel.
        /// </summary>
        /// <param name="kernel">The kernel that is unloading the module.</param>
        public void OnUnload(IKernel kernel)
        {
            Ensure.ArgumentNotNull(kernel, "kernel");
            Unload();
            Bindings.Map(Kernel.RemoveBinding);
            Kernel = null;
        }

        /// <summary>
        ///     Called after loading the modules. A module can verify here if all other required modules are loaded.
        /// </summary>
        public void OnVerifyRequiredModules()
        {
            VerifyRequiredModulesAreLoaded();
        }

        /// <summary>
        ///     Loads the module into the kernel.
        /// </summary>
        public abstract void Load();

        /// <summary>
        ///     Unloads the module from the kernel.
        /// </summary>
        public virtual void Unload()
        {
        }

        /// <summary>
        ///     Called after loading the modules. A module can verify here if all other required modules are loaded.
        /// </summary>
        public virtual void VerifyRequiredModulesAreLoaded()
        {
        }

        /// <summary>
        ///     Unregisters all bindings for the specified service.
        /// </summary>
        /// <param name="service">The service to unbind.</param>
        public override void Unbind(Type service)
        {
            Kernel.Unbind(service);
        }

        /// <summary>
        ///     Registers the specified binding.
        /// </summary>
        /// <param name="binding">The binding to add.</param>
        public override void AddBinding(IBinding binding)
        {
            Ensure.ArgumentNotNull(binding, "binding");

            Kernel.AddBinding(binding);
            Bindings.Add(binding);
        }

        /// <summary>
        ///     Unregisters the specified binding.
        /// </summary>
        /// <param name="binding">The binding to remove.</param>
        public override void RemoveBinding(IBinding binding)
        {
            Ensure.ArgumentNotNull(binding, "binding");

            Kernel.RemoveBinding(binding);
            Bindings.Remove(binding);
        }
    }
}