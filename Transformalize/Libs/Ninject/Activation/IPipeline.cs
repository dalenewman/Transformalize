#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System.Collections.Generic;
using Transformalize.Libs.Ninject.Activation.Strategies;
using Transformalize.Libs.Ninject.Components;

#endregion

namespace Transformalize.Libs.Ninject.Activation
{
    /// <summary>
    ///     Drives the activation (injection, etc.) of an instance.
    /// </summary>
    public interface IPipeline : INinjectComponent
    {
        /// <summary>
        ///     Gets the strategies that contribute to the activation and deactivation processes.
        /// </summary>
        IList<IActivationStrategy> Strategies { get; }

        /// <summary>
        ///     Activates the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">The instance reference.</param>
        void Activate(IContext context, InstanceReference reference);

        /// <summary>
        ///     Deactivates the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">The instance reference.</param>
        void Deactivate(IContext context, InstanceReference reference);
    }
}