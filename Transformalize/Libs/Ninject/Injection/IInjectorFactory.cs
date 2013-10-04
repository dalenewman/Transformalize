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
    ///     Creates injectors from members.
    /// </summary>
    public interface IInjectorFactory : INinjectComponent
    {
        /// <summary>
        ///     Gets or creates an injector for the specified constructor.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <returns>The created injector.</returns>
        ConstructorInjector Create(ConstructorInfo constructor);

        /// <summary>
        ///     Gets or creates an injector for the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The created injector.</returns>
        PropertyInjector Create(PropertyInfo property);

        /// <summary>
        ///     Gets or creates an injector for the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The created injector.</returns>
        MethodInjector Create(MethodInfo method);
    }
}