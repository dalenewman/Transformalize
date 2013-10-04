#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using Transformalize.Libs.Ninject.Infrastructure;

namespace Transformalize.Libs.Ninject.Activation
{
    /// <summary>
    ///     A simple abstract provider for instances of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of instances the provider creates.</typeparam>
    public abstract class Provider<T> : IProvider<T>
    {
        /// <summary>
        ///     Gets the type (or prototype) of instances the provider creates.
        /// </summary>
        public virtual Type Type
        {
            get { return typeof (T); }
        }

        /// <summary>
        ///     Creates an instance within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The created instance.</returns>
        public object Create(IContext context)
        {
            Ensure.ArgumentNotNull(context, "context");
            return CreateInstance(context);
        }

        /// <summary>
        ///     Creates an instance within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The created instance.</returns>
        protected abstract T CreateInstance(IContext context);
    }
}