#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Ninject.Activation.Caching;
using Transformalize.Libs.Ninject.Activation.Strategies;
using Transformalize.Libs.Ninject.Components;
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Infrastructure.Language;

namespace Transformalize.Libs.Ninject.Activation
{
    /// <summary>
    ///     Drives the activation (injection, etc.) of an instance.
    /// </summary>
    public class Pipeline : NinjectComponent, IPipeline
    {
        /// <summary>
        ///     The activation cache.
        /// </summary>
        private readonly IActivationCache activationCache;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Pipeline" /> class.
        /// </summary>
        /// <param name="strategies">The strategies to execute during activation and deactivation.</param>
        /// <param name="activationCache">The activation cache.</param>
        public Pipeline(IEnumerable<IActivationStrategy> strategies, IActivationCache activationCache)
        {
            Ensure.ArgumentNotNull(strategies, "strategies");
            Strategies = strategies.ToList();
            this.activationCache = activationCache;
        }

        /// <summary>
        ///     Gets the strategies that contribute to the activation and deactivation processes.
        /// </summary>
        public IList<IActivationStrategy> Strategies { get; private set; }

        /// <summary>
        ///     Activates the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">The instance reference.</param>
        public void Activate(IContext context, InstanceReference reference)
        {
            Ensure.ArgumentNotNull(context, "context");
            if (!activationCache.IsActivated(reference.Instance))
            {
                Strategies.Map(s => s.Activate(context, reference));
            }
        }

        /// <summary>
        ///     Deactivates the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">The instance reference.</param>
        public void Deactivate(IContext context, InstanceReference reference)
        {
            Ensure.ArgumentNotNull(context, "context");
            if (!activationCache.IsDeactivated(reference.Instance))
            {
                Strategies.Map(s => s.Deactivate(context, reference));
            }
        }
    }
}