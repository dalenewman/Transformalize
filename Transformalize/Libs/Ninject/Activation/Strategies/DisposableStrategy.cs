#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System;

#endregion

namespace Transformalize.Libs.Ninject.Activation.Strategies
{
    /// <summary>
    ///     During deactivation, disposes instances that implement <see cref="IDisposable" />.
    /// </summary>
    public class DisposableStrategy : ActivationStrategy
    {
        /// <summary>
        ///     Disposes the specified instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being deactivated.</param>
        public override void Deactivate(IContext context, InstanceReference reference)
        {
            reference.IfInstanceIs<IDisposable>(x => x.Dispose());
        }
    }
}