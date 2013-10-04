#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System;
using System.Collections.Generic;
using Transformalize.Libs.Ninject.Activation;
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Parameters;

#endregion

namespace Transformalize.Libs.Ninject.Planning.Bindings
{
    /// <summary>
    ///     Contains information about a service registration.
    /// </summary>
    public class Binding : IBinding
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Binding" /> class.
        /// </summary>
        /// <param name="service">The service that is controlled by the binding.</param>
        public Binding(Type service)
        {
            Ensure.ArgumentNotNull(service, "service");

            Service = service;
            BindingConfiguration = new BindingConfiguration();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Binding" /> class.
        /// </summary>
        /// <param name="service">The service that is controlled by the binding.</param>
        /// <param name="configuration">The binding configuration.</param>
        public Binding(Type service, IBindingConfiguration configuration)
        {
            Ensure.ArgumentNotNull(service, "service");
            Ensure.ArgumentNotNull(configuration, "configuration");

            Service = service;
            BindingConfiguration = configuration;
        }

        /// <summary>
        ///     Gets or sets the binding configuration.
        /// </summary>
        /// <value>The binding configuration.</value>
        public IBindingConfiguration BindingConfiguration { get; private set; }

        /// <summary>
        ///     Gets the service type that is controlled by the binding.
        /// </summary>
        public Type Service { get; private set; }

        /// <summary>
        ///     Gets the binding's metadata.
        /// </summary>
        /// <value></value>
        public IBindingMetadata Metadata
        {
            get { return BindingConfiguration.Metadata; }
        }

        /// <summary>
        ///     Gets or sets the type of target for the binding.
        /// </summary>
        /// <value></value>
        public BindingTarget Target
        {
            get { return BindingConfiguration.Target; }

            set { BindingConfiguration.Target = value; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the binding was implicitly registered.
        /// </summary>
        /// <value></value>
        public bool IsImplicit
        {
            get { return BindingConfiguration.IsImplicit; }

            set { BindingConfiguration.IsImplicit = value; }
        }

        /// <summary>
        ///     Gets a value indicating whether the binding has a condition associated with it.
        /// </summary>
        /// <value></value>
        public bool IsConditional
        {
            get { return BindingConfiguration.IsConditional; }
        }

        /// <summary>
        ///     Gets or sets the condition defined for the binding.
        /// </summary>
        /// <value></value>
        public Func<IRequest, bool> Condition
        {
            get { return BindingConfiguration.Condition; }
            set { BindingConfiguration.Condition = value; }
        }

        /// <summary>
        ///     Gets or sets the callback that returns the provider that should be used by the binding.
        /// </summary>
        /// <value></value>
        public Func<IContext, IProvider> ProviderCallback
        {
            get { return BindingConfiguration.ProviderCallback; }

            set { BindingConfiguration.ProviderCallback = value; }
        }

        /// <summary>
        ///     Gets or sets the callback that returns the object that will act as the binding's scope.
        /// </summary>
        /// <value></value>
        public Func<IContext, object> ScopeCallback
        {
            get { return BindingConfiguration.ScopeCallback; }
            set { BindingConfiguration.ScopeCallback = value; }
        }

        /// <summary>
        ///     Gets the parameters defined for the binding.
        /// </summary>
        /// <value></value>
        public ICollection<IParameter> Parameters
        {
            get { return BindingConfiguration.Parameters; }
        }

        /// <summary>
        ///     Gets the actions that should be called after instances are activated via the binding.
        /// </summary>
        /// <value></value>
        public ICollection<Action<IContext, object>> ActivationActions
        {
            get { return BindingConfiguration.ActivationActions; }
        }

        /// <summary>
        ///     Gets the actions that should be called before instances are deactivated via the binding.
        /// </summary>
        /// <value></value>
        public ICollection<Action<IContext, object>> DeactivationActions
        {
            get { return BindingConfiguration.DeactivationActions; }
        }

        /// <summary>
        ///     Gets the provider for the binding.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The provider to use.</returns>
        public IProvider GetProvider(IContext context)
        {
            return BindingConfiguration.GetProvider(context);
        }

        /// <summary>
        ///     Gets the scope for the binding, if any.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///     The object that will act as the scope, or <see langword="null" /> if the service is transient.
        /// </returns>
        public object GetScope(IContext context)
        {
            return BindingConfiguration.GetScope(context);
        }

        /// <summary>
        ///     Determines whether the specified request satisfies the condition defined on the binding,
        ///     if one was defined.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///     <c>True</c> if the request satisfies the condition; otherwise <c>false</c>.
        /// </returns>
        public bool Matches(IRequest request)
        {
            return BindingConfiguration.Matches(request);
        }
    }
}