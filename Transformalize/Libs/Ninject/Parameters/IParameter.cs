#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System;
using Transformalize.Libs.Ninject.Activation;
using Transformalize.Libs.Ninject.Planning.Targets;

#endregion

namespace Transformalize.Libs.Ninject.Parameters
{
    /// <summary>
    ///     Modifies an activation process in some way.
    /// </summary>
    public interface IParameter : IEquatable<IParameter>
    {
        /// <summary>
        ///     Gets the name of the parameter.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets a value indicating whether the parameter should be inherited into child requests.
        /// </summary>
        bool ShouldInherit { get; }

        /// <summary>
        ///     Gets the value for the parameter within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>The value for the parameter.</returns>
        object GetValue(IContext context, ITarget target);
    }
}