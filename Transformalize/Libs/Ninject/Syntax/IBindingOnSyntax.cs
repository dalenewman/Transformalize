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
    ///     Used to add additional actions to be performed during activation or deactivation of instances via a binding.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    public interface IBindingOnSyntax<T> : IBindingSyntax
    {
        /// <summary>
        ///     Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingOnSyntax<T> OnActivation(Action<T> action);

        /// <summary>
        ///     Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingOnSyntax<T> OnActivation<TImplementation>(Action<TImplementation> action);

        /// <summary>
        ///     Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingOnSyntax<T> OnActivation(Action<IContext, T> action);

        /// <summary>
        ///     Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingOnSyntax<T> OnActivation<TImplementation>(Action<IContext, TImplementation> action);

        /// <summary>
        ///     Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingOnSyntax<T> OnDeactivation(Action<T> action);

        /// <summary>
        ///     Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingOnSyntax<T> OnDeactivation<TImplementation>(Action<TImplementation> action);

        /// <summary>
        ///     Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingOnSyntax<T> OnDeactivation(Action<IContext, T> action);

        /// <summary>
        ///     Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingOnSyntax<T> OnDeactivation<TImplementation>(Action<IContext, TImplementation> action);
    }
}