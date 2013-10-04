#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System.Reflection;
using Transformalize.Libs.Ninject.Injection;
using Transformalize.Libs.Ninject.Planning.Targets;

#endregion

namespace Transformalize.Libs.Ninject.Planning.Directives
{
    /// <summary>
    ///     Describes the injection of a property.
    /// </summary>
    public class PropertyInjectionDirective : IDirective
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyInjectionDirective" /> class.
        /// </summary>
        /// <param name="member">The member the directive describes.</param>
        /// <param name="injector">The injector that will be triggered.</param>
        public PropertyInjectionDirective(PropertyInfo member, PropertyInjector injector)
        {
            Injector = injector;
            Target = CreateTarget(member);
        }

        /// <summary>
        ///     Gets or sets the injector that will be triggered.
        /// </summary>
        public PropertyInjector Injector { get; private set; }

        /// <summary>
        ///     Gets or sets the injection target for the directive.
        /// </summary>
        public ITarget Target { get; private set; }

        /// <summary>
        ///     Creates a target for the property.
        /// </summary>
        /// <param name="propertyInfo">The property.</param>
        /// <returns>The target for the property.</returns>
        protected virtual ITarget CreateTarget(PropertyInfo propertyInfo)
        {
            return new PropertyTarget(propertyInfo);
        }
    }
}