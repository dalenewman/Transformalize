#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using Transformalize.Libs.Ninject.Activation;
using Transformalize.Libs.Ninject.Planning.Targets;

namespace Transformalize.Libs.Ninject.Parameters
{
    /// <summary>
    ///     Overrides the injected value of a constructor argument.
    /// </summary>
    public class ConstructorArgument : Parameter, IConstructorArgument
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConstructorArgument" /> class.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="value">The value to inject into the property.</param>
        public ConstructorArgument(string name, object value)
            : base(name, value, false)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConstructorArgument" /> class.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="valueCallback">The callback to invoke to get the value that should be injected.</param>
        public ConstructorArgument(string name, Func<IContext, object> valueCallback)
            : base(name, valueCallback, false)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConstructorArgument" /> class.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="valueCallback">The callback to invoke to get the value that should be injected.</param>
        public ConstructorArgument(string name, Func<IContext, ITarget, object> valueCallback)
            : base(name, valueCallback, false)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConstructorArgument" /> class.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="value">The value to inject into the property.</param>
        /// <param name="shouldInherit">Whether the parameter should be inherited into child requests.</param>
        public ConstructorArgument(string name, object value, bool shouldInherit)
            : base(name, value, shouldInherit)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConstructorArgument" /> class.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="valueCallback">The callback to invoke to get the value that should be injected.</param>
        /// <param name="shouldInherit">
        ///     if set to <c>true</c> [should inherit].
        /// </param>
        public ConstructorArgument(string name, Func<IContext, object> valueCallback, bool shouldInherit)
            : base(name, valueCallback, shouldInherit)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConstructorArgument" /> class.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="valueCallback">The callback to invoke to get the value that should be injected.</param>
        /// <param name="shouldInherit">
        ///     if set to <c>true</c> [should inherit].
        /// </param>
        public ConstructorArgument(string name, Func<IContext, ITarget, object> valueCallback, bool shouldInherit)
            : base(name, valueCallback, shouldInherit)
        {
        }

        /// <summary>
        ///     Determines if the parameter applies to the given target.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>
        ///     Tre if the parameter applies in the specified context to the specified target.
        /// </returns>
        /// <remarks>
        ///     Only one parameter may return true.
        /// </remarks>
        public bool AppliesToTarget(IContext context, ITarget target)
        {
            return string.Equals(Name, target.Name);
        }
    }
}