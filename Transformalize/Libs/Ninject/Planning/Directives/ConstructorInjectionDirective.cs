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
    ///     Describes the injection of a constructor.
    /// </summary>
    public class ConstructorInjectionDirective : MethodInjectionDirectiveBase<ConstructorInfo, ConstructorInjector>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConstructorInjectionDirective" /> class.
        /// </summary>
        /// <param name="constructor">The constructor described by the directive.</param>
        /// <param name="injector">The injector that will be triggered.</param>
        public ConstructorInjectionDirective(ConstructorInfo constructor, ConstructorInjector injector)
            : base(constructor, injector)
        {
            Constructor = constructor;
        }

        /// <summary>
        ///     The base .ctor definition.
        /// </summary>
        public ConstructorInfo Constructor { get; set; }
    }
}