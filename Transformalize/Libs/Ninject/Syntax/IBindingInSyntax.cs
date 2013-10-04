#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using Transformalize.Libs.Ninject.Activation;

namespace Transformalize.Libs.Ninject.Syntax
{
    /// <summary>
    ///     Used to define the scope in which instances activated via a binding should be re-used.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    public interface IBindingInSyntax<T> : IBindingSyntax
    {
        /// <summary>
        ///     Indicates that only a single instance of the binding should be created, and then
        ///     should be re-used for all subsequent requests.
        /// </summary>
        /// <returns>The fluent syntax.</returns>
        IBindingNamedWithOrOnSyntax<T> InSingletonScope();

        /// <summary>
        ///     Indicates that instances activated via the binding should not be re-used, nor have
        ///     their lifecycle managed by Ninject.
        /// </summary>
        /// <returns>The fluent syntax.</returns>
        IBindingNamedWithOrOnSyntax<T> InTransientScope();

        /// <summary>
        ///     Indicates that instances activated via the binding should be re-used within the same thread.
        /// </summary>
        /// <returns>The fluent syntax.</returns>
        IBindingNamedWithOrOnSyntax<T> InThreadScope();

        /// <summary>
        ///     Indicates that instances activated via the binding should be re-used as long as the object
        ///     returned by the provided callback remains alive (that is, has not been garbage collected).
        /// </summary>
        /// <param name="scope">The callback that returns the scope.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingNamedWithOrOnSyntax<T> InScope(Func<IContext, object> scope);
    }
}