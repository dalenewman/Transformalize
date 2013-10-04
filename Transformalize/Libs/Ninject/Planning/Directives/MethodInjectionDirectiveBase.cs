#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System.Linq;
using System.Reflection;
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Planning.Targets;

#endregion

namespace Transformalize.Libs.Ninject.Planning.Directives
{
    /// <summary>
    ///     Describes the injection of a method or constructor.
    /// </summary>
    public abstract class MethodInjectionDirectiveBase<TMethod, TInjector> : IDirective
        where TMethod : MethodBase
    {
        /// <summary>
        ///     Initializes a new instance of the MethodInjectionDirectiveBase&lt;TMethod, TInjector&gt; class.
        /// </summary>
        /// <param name="method">The method this directive represents.</param>
        /// <param name="injector">The injector that will be triggered.</param>
        protected MethodInjectionDirectiveBase(TMethod method, TInjector injector)
        {
            Ensure.ArgumentNotNull(method, "method");
            Ensure.ArgumentNotNull(injector, "injector");

            Injector = injector;
            Targets = CreateTargetsFromParameters(method);
        }

        /// <summary>
        ///     Gets or sets the injector that will be triggered.
        /// </summary>
        public TInjector Injector { get; private set; }

        /// <summary>
        ///     Gets or sets the targets for the directive.
        /// </summary>
        public ITarget[] Targets { get; private set; }

        /// <summary>
        ///     Creates targets for the parameters of the method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The targets for the method's parameters.</returns>
        protected virtual ITarget[] CreateTargetsFromParameters(TMethod method)
        {
            return method.GetParameters().Select(parameter => new ParameterTarget(method, parameter)).ToArray();
        }
    }
}