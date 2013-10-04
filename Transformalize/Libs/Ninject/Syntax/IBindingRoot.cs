#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using Transformalize.Libs.Ninject.Planning.Bindings;

namespace Transformalize.Libs.Ninject.Syntax
{
    /// <summary>
    ///     Provides a path to register bindings.
    /// </summary>
    public interface IBindingRoot : IFluentSyntax
    {
        /// <summary>
        ///     Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T">The service to bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        IBindingToSyntax<T> Bind<T>();

        /// <summary>
        ///     Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T1">The first service to bind.</typeparam>
        /// <typeparam name="T2">The second service to bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        IBindingToSyntax<T1, T2> Bind<T1, T2>();

        /// <summary>
        ///     Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T1">The first service to bind.</typeparam>
        /// <typeparam name="T2">The second service to bind.</typeparam>
        /// <typeparam name="T3">The third service to bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        IBindingToSyntax<T1, T2, T3> Bind<T1, T2, T3>();

        /// <summary>
        ///     Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T1">The first service to bind.</typeparam>
        /// <typeparam name="T2">The second service to bind.</typeparam>
        /// <typeparam name="T3">The third service to bind.</typeparam>
        /// <typeparam name="T4">The fourth service to bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        IBindingToSyntax<T1, T2, T3, T4> Bind<T1, T2, T3, T4>();

        /// <summary>
        ///     Declares a binding from the service to itself.
        /// </summary>
        /// <param name="services">The services to bind.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingToSyntax<object> Bind(params Type[] services);

        /// <summary>
        ///     Unregisters all bindings for the specified service.
        /// </summary>
        /// <typeparam name="T">The service to unbind.</typeparam>
        void Unbind<T>();

        /// <summary>
        ///     Unregisters all bindings for the specified service.
        /// </summary>
        /// <param name="service">The service to unbind.</param>
        void Unbind(Type service);

        /// <summary>
        ///     Removes any existing bindings for the specified service, and declares a new one.
        /// </summary>
        /// <typeparam name="T1">The first service to re-bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        IBindingToSyntax<T1> Rebind<T1>();

        /// <summary>
        ///     Removes any existing bindings for the specified services, and declares a new one.
        /// </summary>
        /// <typeparam name="T1">The first service to re-bind.</typeparam>
        /// <typeparam name="T2">The second service to re-bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        IBindingToSyntax<T1, T2> Rebind<T1, T2>();

        /// <summary>
        ///     Removes any existing bindings for the specified services, and declares a new one.
        /// </summary>
        /// <typeparam name="T1">The first service to re-bind.</typeparam>
        /// <typeparam name="T2">The second service to re-bind.</typeparam>
        /// <typeparam name="T3">The third service to re-bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        IBindingToSyntax<T1, T2, T3> Rebind<T1, T2, T3>();

        /// <summary>
        ///     Removes any existing bindings for the specified services, and declares a new one.
        /// </summary>
        /// <typeparam name="T1">The first service to re-bind.</typeparam>
        /// <typeparam name="T2">The second service to re-bind.</typeparam>
        /// <typeparam name="T3">The third service to re-bind.</typeparam>
        /// <typeparam name="T4">The fourth service to re-bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        IBindingToSyntax<T1, T2, T3, T4> Rebind<T1, T2, T3, T4>();

        /// <summary>
        ///     Removes any existing bindings for the specified services, and declares a new one.
        /// </summary>
        /// <param name="services">The services to re-bind.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingToSyntax<object> Rebind(params Type[] services);

        /// <summary>
        ///     Registers the specified binding.
        /// </summary>
        /// <param name="binding">The binding to add.</param>
        void AddBinding(IBinding binding);

        /// <summary>
        ///     Unregisters the specified binding.
        /// </summary>
        /// <param name="binding">The binding to remove.</param>
        void RemoveBinding(IBinding binding);
    }
}