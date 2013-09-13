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

using Transformalize.Libs.Ninject.Activation;
using Transformalize.Libs.Ninject.Activation.Caching;
using Transformalize.Libs.Ninject.Activation.Strategies;
using Transformalize.Libs.Ninject.Injection;
using Transformalize.Libs.Ninject.Modules;
using Transformalize.Libs.Ninject.Planning;
using Transformalize.Libs.Ninject.Planning.Bindings.Resolvers;
using Transformalize.Libs.Ninject.Planning.Strategies;
using Transformalize.Libs.Ninject.Selection;
using Transformalize.Libs.Ninject.Selection.Heuristics;

namespace Transformalize.Libs.Ninject
{
    /// <summary>
    ///     The standard implementation of a kernel.
    /// </summary>
    public class StandardKernel : KernelBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StandardKernel" /> class.
        /// </summary>
        /// <param name="modules">The modules to load into the kernel.</param>
        public StandardKernel(params INinjectModule[] modules) : base(modules)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StandardKernel" /> class.
        /// </summary>
        /// <param name="settings">The configuration to use.</param>
        /// <param name="modules">The modules to load into the kernel.</param>
        public StandardKernel(INinjectSettings settings, params INinjectModule[] modules) : base(settings, modules)
        {
        }

        /// <summary>
        ///     Gets the kernel.
        /// </summary>
        /// <value>The kernel.</value>
        protected override IKernel KernelInstance
        {
            get { return this; }
        }

        /// <summary>
        ///     Adds components to the kernel during startup.
        /// </summary>
        protected override void AddComponents()
        {
            Components.Add<IPlanner, Planner>();
            Components.Add<IPlanningStrategy, ConstructorReflectionStrategy>();
            Components.Add<IPlanningStrategy, PropertyReflectionStrategy>();
            Components.Add<IPlanningStrategy, MethodReflectionStrategy>();

            Components.Add<ISelector, Selector>();
            Components.Add<IConstructorScorer, StandardConstructorScorer>();
            Components.Add<IInjectionHeuristic, StandardInjectionHeuristic>();

            Components.Add<IPipeline, Pipeline>();
            if (!Settings.ActivationCacheDisabled)
            {
                Components.Add<IActivationStrategy, ActivationCacheStrategy>();
            }

            Components.Add<IActivationStrategy, PropertyInjectionStrategy>();
            Components.Add<IActivationStrategy, MethodInjectionStrategy>();
            Components.Add<IActivationStrategy, InitializableStrategy>();
            Components.Add<IActivationStrategy, StartableStrategy>();
            Components.Add<IActivationStrategy, BindingActionStrategy>();
            Components.Add<IActivationStrategy, DisposableStrategy>();

            Components.Add<IBindingResolver, StandardBindingResolver>();
            Components.Add<IBindingResolver, OpenGenericBindingResolver>();

            Components.Add<IMissingBindingResolver, DefaultValueBindingResolver>();
            Components.Add<IMissingBindingResolver, SelfBindingResolver>();

#if !NO_LCG
            if (!Settings.UseReflectionBasedInjection)
            {
                Components.Add<IInjectorFactory, DynamicMethodInjectorFactory>();
            }
            else
#endif
            {
                Components.Add<IInjectorFactory, ReflectionInjectorFactory>();
            }

            Components.Add<ICache, Cache>();
            Components.Add<IActivationCache, ActivationCache>();
            Components.Add<ICachePruner, GarbageCollectionCachePruner>();

#if !NO_ASSEMBLY_SCANNING
            Components.Add<IModuleLoader, ModuleLoader>();
            Components.Add<IModuleLoaderPlugin, CompiledModuleLoaderPlugin>();
            Components.Add<IAssemblyNameRetriever, AssemblyNameRetriever>();
#endif
        }
    }
}