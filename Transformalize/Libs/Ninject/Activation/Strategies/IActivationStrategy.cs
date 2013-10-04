#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using Transformalize.Libs.Ninject.Components;

#endregion

namespace Transformalize.Libs.Ninject.Activation.Strategies
{
    /// <summary>
    ///     Contributes to a <see cref="IPipeline" />, and is called during the activation
    ///     and deactivation of an instance.
    /// </summary>
    public interface IActivationStrategy : INinjectComponent
    {
        /// <summary>
        ///     Contributes to the activation of the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being activated.</param>
        void Activate(IContext context, InstanceReference reference);

        /// <summary>
        ///     Contributes to the deactivation of the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being deactivated.</param>
        void Deactivate(IContext context, InstanceReference reference);
    }
}