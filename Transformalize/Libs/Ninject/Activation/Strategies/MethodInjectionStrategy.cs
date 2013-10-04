#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System.Linq;
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Planning.Directives;

#endregion

namespace Transformalize.Libs.Ninject.Activation.Strategies
{
    /// <summary>
    ///     Injects methods on an instance during activation.
    /// </summary>
    public class MethodInjectionStrategy : ActivationStrategy
    {
        /// <summary>
        ///     Injects values into the properties as described by <see cref="MethodInjectionDirective" />s
        ///     contained in the plan.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being activated.</param>
        public override void Activate(IContext context, InstanceReference reference)
        {
            Ensure.ArgumentNotNull(context, "context");
            Ensure.ArgumentNotNull(reference, "reference");

            foreach (var directive in context.Plan.GetAll<MethodInjectionDirective>())
            {
                var arguments = directive.Targets.Select(target => target.ResolveWithin(context));
                directive.Injector(reference.Instance, arguments.ToArray());
            }
        }
    }
}