#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
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