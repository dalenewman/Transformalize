#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

#endregion

namespace Transformalize.Libs.Ninject.Activation.Strategies
{
    /// <summary>
    ///     Starts instances that implement <see cref="IStartable" /> during activation,
    ///     and stops them during deactivation.
    /// </summary>
    public class StartableStrategy : ActivationStrategy
    {
        /// <summary>
        ///     Starts the specified instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being activated.</param>
        public override void Activate(IContext context, InstanceReference reference)
        {
            reference.IfInstanceIs<IStartable>(x => x.Start());
        }

        /// <summary>
        ///     Stops the specified instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being deactivated.</param>
        public override void Deactivate(IContext context, InstanceReference reference)
        {
            reference.IfInstanceIs<IStartable>(x => x.Stop());
        }
    }
}