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
    ///     During activation, initializes instances that implement <see cref="IInitializable" />.
    /// </summary>
    public class InitializableStrategy : ActivationStrategy
    {
        /// <summary>
        ///     Initializes the specified instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being activated.</param>
        public override void Activate(IContext context, InstanceReference reference)
        {
            reference.IfInstanceIs<IInitializable>(x => x.Initialize());
        }
    }
}