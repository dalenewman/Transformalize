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

#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Ninject.Activation.Caching;
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Infrastructure.Introspection;
using Transformalize.Libs.Ninject.Parameters;
using Transformalize.Libs.Ninject.Planning;
using Transformalize.Libs.Ninject.Planning.Bindings;

#endregion

namespace Transformalize.Libs.Ninject.Activation
{
    /// <summary>
    ///     Contains information about the activation of a single instance.
    /// </summary>
    public class Context : IContext
    {
        private WeakReference cachedScope;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Context" /> class.
        /// </summary>
        /// <param name="kernel">The kernel managing the resolution.</param>
        /// <param name="request">The context's request.</param>
        /// <param name="binding">The context's binding.</param>
        /// <param name="cache">The cache component.</param>
        /// <param name="planner">The planner component.</param>
        /// <param name="pipeline">The pipeline component.</param>
        public Context(IKernel kernel, IRequest request, IBinding binding, ICache cache, IPlanner planner, IPipeline pipeline)
        {
            Ensure.ArgumentNotNull(kernel, "kernel");
            Ensure.ArgumentNotNull(request, "request");
            Ensure.ArgumentNotNull(binding, "binding");
            Ensure.ArgumentNotNull(cache, "cache");
            Ensure.ArgumentNotNull(planner, "planner");
            Ensure.ArgumentNotNull(pipeline, "pipeline");

            Kernel = kernel;
            Request = request;
            Binding = binding;
            Parameters = request.Parameters.Union(binding.Parameters).ToList();

            Cache = cache;
            Planner = planner;
            Pipeline = pipeline;

            if (binding.Service.IsGenericTypeDefinition)
            {
                HasInferredGenericArguments = true;
                GenericArguments = request.Service.GetGenericArguments();
            }
        }

        /// <summary>
        ///     Gets or sets the cache component.
        /// </summary>
        public ICache Cache { get; private set; }

        /// <summary>
        ///     Gets or sets the planner component.
        /// </summary>
        public IPlanner Planner { get; private set; }

        /// <summary>
        ///     Gets or sets the pipeline component.
        /// </summary>
        public IPipeline Pipeline { get; private set; }

        /// <summary>
        ///     Gets the kernel that is driving the activation.
        /// </summary>
        public IKernel Kernel { get; set; }

        /// <summary>
        ///     Gets the request.
        /// </summary>
        public IRequest Request { get; set; }

        /// <summary>
        ///     Gets the binding.
        /// </summary>
        public IBinding Binding { get; set; }

        /// <summary>
        ///     Gets or sets the activation plan.
        /// </summary>
        public IPlan Plan { get; set; }

        /// <summary>
        ///     Gets the parameters that were passed to manipulate the activation process.
        /// </summary>
        public ICollection<IParameter> Parameters { get; set; }

        /// <summary>
        ///     Gets the generic arguments for the request, if any.
        /// </summary>
        public Type[] GenericArguments { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the request involves inferred generic arguments.
        /// </summary>
        public bool HasInferredGenericArguments { get; private set; }

        /// <summary>
        ///     Gets the scope for the context that "owns" the instance activated therein.
        /// </summary>
        /// <returns>The object that acts as the scope.</returns>
        public object GetScope()
        {
            if (cachedScope == null)
            {
                var scope = Request.GetScope() ?? Binding.GetScope(this);
                cachedScope = new WeakReference(scope);
            }

            return cachedScope.Target;
        }

        /// <summary>
        ///     Gets the provider that should be used to create the instance for this context.
        /// </summary>
        /// <returns>The provider that should be used.</returns>
        public IProvider GetProvider()
        {
            return Binding.GetProvider(this);
        }

        /// <summary>
        ///     Resolves the instance associated with this hook.
        /// </summary>
        /// <returns>The resolved instance.</returns>
        public object Resolve()
        {
            lock (Binding)
            {
                if (Request.ActiveBindings.Contains(Binding))
                    throw new ActivationException(ExceptionFormatter.CyclicalDependenciesDetected(this));

                var cachedInstance = Cache.TryGet(this);

                if (cachedInstance != null)
                    return cachedInstance;

                Request.ActiveBindings.Push(Binding);

                var reference = new InstanceReference
                                    {
                                        Instance = GetProvider().Create(this)
                                    };

                Request.ActiveBindings.Pop();

                if (reference.Instance == null)
                {
                    if (!Kernel.Settings.AllowNullInjection)
                    {
                        throw new ActivationException(ExceptionFormatter.ProviderReturnedNull(this));
                    }

                    if (Plan == null)
                    {
                        Plan = Planner.GetPlan(Request.Service);
                    }

                    return null;
                }

                if (GetScope() != null)
                    Cache.Remember(this, reference);

                if (Plan == null)
                    Plan = Planner.GetPlan(reference.Instance.GetType());

                Pipeline.Activate(this, reference);

                return reference.Instance;
            }
        }
    }
}