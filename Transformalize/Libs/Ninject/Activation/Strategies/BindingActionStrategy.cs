#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Infrastructure.Language;

#endregion

namespace Transformalize.Libs.Ninject.Activation.Strategies
{
    /// <summary>
    ///     Executes actions defined on the binding during activation and deactivation.
    /// </summary>
    public class BindingActionStrategy : ActivationStrategy
    {
        /// <summary>
        ///     Calls the activation actions defined on the binding.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being activated.</param>
        public override void Activate(IContext context, InstanceReference reference)
        {
            Ensure.ArgumentNotNull(context, "context");
            context.Binding.ActivationActions.Map(action => action(context, reference.Instance));
        }

        /// <summary>
        ///     Calls the deactivation actions defined on the binding.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being deactivated.</param>
        public override void Deactivate(IContext context, InstanceReference reference)
        {
            Ensure.ArgumentNotNull(context, "context");
            context.Binding.DeactivationActions.Map(action => action(context, reference.Instance));
        }
    }
}