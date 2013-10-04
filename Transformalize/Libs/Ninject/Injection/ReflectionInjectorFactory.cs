#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System.Reflection;
using Transformalize.Libs.Ninject.Components;

#endregion

namespace Transformalize.Libs.Ninject.Injection
{
    /// <summary>
    ///     Creates injectors from members via reflective invocation.
    /// </summary>
    public class ReflectionInjectorFactory : NinjectComponent, IInjectorFactory
    {
        /// <summary>
        ///     Gets or creates an injector for the specified constructor.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <returns>The created injector.</returns>
        public ConstructorInjector Create(ConstructorInfo constructor)
        {
            return args => constructor.Invoke(args);
        }

        /// <summary>
        ///     Gets or creates an injector for the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The created injector.</returns>
        public PropertyInjector Create(PropertyInfo property)
        {
            return (target, value) => property.SetValue(target, value, null);
        }

        /// <summary>
        ///     Gets or creates an injector for the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The created injector.</returns>
        public MethodInjector Create(MethodInfo method)
        {
            return (target, args) => method.Invoke(target, args);
        }
    }
}