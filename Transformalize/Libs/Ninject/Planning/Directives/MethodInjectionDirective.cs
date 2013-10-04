#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System.Reflection;
using Transformalize.Libs.Ninject.Injection;

#endregion

namespace Transformalize.Libs.Ninject.Planning.Directives
{
    /// <summary>
    ///     Describes the injection of a method.
    /// </summary>
    public class MethodInjectionDirective : MethodInjectionDirectiveBase<MethodInfo, MethodInjector>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MethodInjectionDirective" /> class.
        /// </summary>
        /// <param name="method">The method described by the directive.</param>
        /// <param name="injector">The injector that will be triggered.</param>
        public MethodInjectionDirective(MethodInfo method, MethodInjector injector)
            : base(method, injector)
        {
        }
    }
}