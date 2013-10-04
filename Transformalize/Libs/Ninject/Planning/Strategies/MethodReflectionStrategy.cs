#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using Transformalize.Libs.Ninject.Components;
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Injection;
using Transformalize.Libs.Ninject.Planning.Directives;
using Transformalize.Libs.Ninject.Selection;

#endregion

namespace Transformalize.Libs.Ninject.Planning.Strategies
{
    /// <summary>
    ///     Adds directives to plans indicating which methods should be injected during activation.
    /// </summary>
    public class MethodReflectionStrategy : NinjectComponent, IPlanningStrategy
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MethodReflectionStrategy" /> class.
        /// </summary>
        /// <param name="selector">The selector component.</param>
        /// <param name="injectorFactory">The injector factory component.</param>
        public MethodReflectionStrategy(ISelector selector, IInjectorFactory injectorFactory)
        {
            Ensure.ArgumentNotNull(selector, "selector");
            Ensure.ArgumentNotNull(injectorFactory, "injectorFactory");

            Selector = selector;
            InjectorFactory = injectorFactory;
        }

        /// <summary>
        ///     Gets the selector component.
        /// </summary>
        public ISelector Selector { get; private set; }

        /// <summary>
        ///     Gets the injector factory component.
        /// </summary>
        public IInjectorFactory InjectorFactory { get; set; }

        /// <summary>
        ///     Adds a <see cref="MethodInjectionDirective" /> to the plan for each method
        ///     that should be injected.
        /// </summary>
        /// <param name="plan">The plan that is being generated.</param>
        public void Execute(IPlan plan)
        {
            Ensure.ArgumentNotNull(plan, "plan");

            foreach (var method in Selector.SelectMethodsForInjection(plan.Type))
                plan.Add(new MethodInjectionDirective(method, InjectorFactory.Create(method)));
        }
    }
}