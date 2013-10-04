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
    public class WeakConstructorArgument : Parameter, IConstructorArgument
    {
        /// <summary>
        ///     A weak reference to the constructor argument value.
        /// </summary>
        private readonly WeakReference weakReference;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConstructorArgument" /> class.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="value">The value to inject into the property.</param>
        public WeakConstructorArgument(string name, object value)
            : this(name, value, false)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConstructorArgument" /> class.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="value">The value to inject into the property.</param>
        /// <param name="shouldInherit">Whether the parameter should be inherited into child requests.</param>
        public WeakConstructorArgument(string name, object value, bool shouldInherit)
            : base(name, value, shouldInherit)
        {
            weakReference = new WeakReference(value);
            ValueCallback = (ctx, target) => weakReference.Target;
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