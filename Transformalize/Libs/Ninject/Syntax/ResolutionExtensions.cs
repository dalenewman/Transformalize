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

#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Parameters;
using Transformalize.Libs.Ninject.Planning.Bindings;

#endregion

namespace Transformalize.Libs.Ninject.Syntax
{
    /// <summary>
    ///     Extensions that enhance resolution of services.
    /// </summary>
    public static class ResolutionExtensions
    {
        /// <summary>
        ///     Gets an instance of the specified service.
        /// </summary>
        /// <typeparam name="T">The service to resolve.</typeparam>
        /// <param name="root">The resolution root.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>An instance of the service.</returns>
        public static T Get<T>(this IResolutionRoot root, params IParameter[] parameters)
        {
            return GetResolutionIterator(root, typeof (T), null, parameters, false, true).Cast<T>().Single();
        }

        /// <summary>
        ///     Gets an instance of the specified service by using the first binding with the specified name.
        /// </summary>
        /// <typeparam name="T">The service to resolve.</typeparam>
        /// <param name="root">The resolution root.</param>
        /// <param name="name">The name of the binding.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>An instance of the service.</returns>
        public static T Get<T>(this IResolutionRoot root, string name, params IParameter[] parameters)
        {
            return GetResolutionIterator(root, typeof (T), b => b.Name == name, parameters, false, true).Cast<T>().Single();
        }

        /// <summary>
        ///     Gets an instance of the specified service by using the first binding that matches the specified constraint.
        /// </summary>
        /// <typeparam name="T">The service to resolve.</typeparam>
        /// <param name="root">The resolution root.</param>
        /// <param name="constraint">The constraint to apply to the binding.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>An instance of the service.</returns>
        public static T Get<T>(this IResolutionRoot root, Func<IBindingMetadata, bool> constraint, params IParameter[] parameters)
        {
            return GetResolutionIterator(root, typeof (T), constraint, parameters, false, true).Cast<T>().Single();
        }

        /// <summary>
        ///     Tries to get an instance of the specified service.
        /// </summary>
        /// <typeparam name="T">The service to resolve.</typeparam>
        /// <param name="root">The resolution root.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>
        ///     An instance of the service, or <see langword="null" /> if no implementation was available.
        /// </returns>
        public static T TryGet<T>(this IResolutionRoot root, params IParameter[] parameters)
        {
            return TryGet(GetResolutionIterator(root, typeof (T), null, parameters, true, true).Cast<T>());
        }

        /// <summary>
        ///     Tries to get an instance of the specified service by using the first binding with the specified name.
        /// </summary>
        /// <typeparam name="T">The service to resolve.</typeparam>
        /// <param name="root">The resolution root.</param>
        /// <param name="name">The name of the binding.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>
        ///     An instance of the service, or <see langword="null" /> if no implementation was available.
        /// </returns>
        public static T TryGet<T>(this IResolutionRoot root, string name, params IParameter[] parameters)
        {
            return TryGet(GetResolutionIterator(root, typeof (T), b => b.Name == name, parameters, true, true).Cast<T>());
        }

        /// <summary>
        ///     Tries to get an instance of the specified service by using the first binding that matches the specified constraint.
        /// </summary>
        /// <typeparam name="T">The service to resolve.</typeparam>
        /// <param name="root">The resolution root.</param>
        /// <param name="constraint">The constraint to apply to the binding.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>
        ///     An instance of the service, or <see langword="null" /> if no implementation was available.
        /// </returns>
        public static T TryGet<T>(this IResolutionRoot root, Func<IBindingMetadata, bool> constraint, params IParameter[] parameters)
        {
            return TryGet(GetResolutionIterator(root, typeof (T), constraint, parameters, true, true).Cast<T>());
        }

        /// <summary>
        ///     Gets all available instances of the specified service.
        /// </summary>
        /// <typeparam name="T">The service to resolve.</typeparam>
        /// <param name="root">The resolution root.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>A series of instances of the service.</returns>
        public static IEnumerable<T> GetAll<T>(this IResolutionRoot root, params IParameter[] parameters)
        {
            return GetResolutionIterator(root, typeof (T), null, parameters, true, false).Cast<T>();
        }

        /// <summary>
        ///     Gets all instances of the specified service using bindings registered with the specified name.
        /// </summary>
        /// <typeparam name="T">The service to resolve.</typeparam>
        /// <param name="root">The resolution root.</param>
        /// <param name="name">The name of the binding.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>A series of instances of the service.</returns>
        public static IEnumerable<T> GetAll<T>(this IResolutionRoot root, string name, params IParameter[] parameters)
        {
            return GetResolutionIterator(root, typeof (T), b => b.Name == name, parameters, true, false).Cast<T>();
        }

        /// <summary>
        ///     Gets all instances of the specified service by using the bindings that match the specified constraint.
        /// </summary>
        /// <typeparam name="T">The service to resolve.</typeparam>
        /// <param name="root">The resolution root.</param>
        /// <param name="constraint">The constraint to apply to the bindings.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>A series of instances of the service.</returns>
        public static IEnumerable<T> GetAll<T>(this IResolutionRoot root, Func<IBindingMetadata, bool> constraint, params IParameter[] parameters)
        {
            return GetResolutionIterator(root, typeof (T), constraint, parameters, true, false).Cast<T>();
        }

        /// <summary>
        ///     Gets an instance of the specified service.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="service">The service to resolve.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>An instance of the service.</returns>
        public static object Get(this IResolutionRoot root, Type service, params IParameter[] parameters)
        {
            return GetResolutionIterator(root, service, null, parameters, false, true).Single();
        }

        /// <summary>
        ///     Gets an instance of the specified service by using the first binding with the specified name.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="service">The service to resolve.</param>
        /// <param name="name">The name of the binding.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>An instance of the service.</returns>
        public static object Get(this IResolutionRoot root, Type service, string name, params IParameter[] parameters)
        {
            return GetResolutionIterator(root, service, b => b.Name == name, parameters, false, true).Single();
        }

        /// <summary>
        ///     Gets an instance of the specified service by using the first binding that matches the specified constraint.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="service">The service to resolve.</param>
        /// <param name="constraint">The constraint to apply to the binding.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>An instance of the service.</returns>
        public static object Get(this IResolutionRoot root, Type service, Func<IBindingMetadata, bool> constraint, params IParameter[] parameters)
        {
            return GetResolutionIterator(root, service, constraint, parameters, false, true).Single();
        }

        /// <summary>
        ///     Tries to get an instance of the specified service.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="service">The service to resolve.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>
        ///     An instance of the service, or <see langword="null" /> if no implementation was available.
        /// </returns>
        public static object TryGet(this IResolutionRoot root, Type service, params IParameter[] parameters)
        {
            return TryGet(GetResolutionIterator(root, service, null, parameters, true, true));
        }

        /// <summary>
        ///     Tries to get an instance of the specified service by using the first binding with the specified name.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="service">The service to resolve.</param>
        /// <param name="name">The name of the binding.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>
        ///     An instance of the service, or <see langword="null" /> if no implementation was available.
        /// </returns>
        public static object TryGet(this IResolutionRoot root, Type service, string name, params IParameter[] parameters)
        {
            return TryGet(GetResolutionIterator(root, service, b => b.Name == name, parameters, true, false));
        }

        /// <summary>
        ///     Tries to get an instance of the specified service by using the first binding that matches the specified constraint.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="service">The service to resolve.</param>
        /// <param name="constraint">The constraint to apply to the binding.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>
        ///     An instance of the service, or <see langword="null" /> if no implementation was available.
        /// </returns>
        public static object TryGet(this IResolutionRoot root, Type service, Func<IBindingMetadata, bool> constraint, params IParameter[] parameters)
        {
            return TryGet(GetResolutionIterator(root, service, constraint, parameters, true, false));
        }

        /// <summary>
        ///     Gets all available instances of the specified service.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="service">The service to resolve.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>A series of instances of the service.</returns>
        public static IEnumerable<object> GetAll(this IResolutionRoot root, Type service, params IParameter[] parameters)
        {
            return GetResolutionIterator(root, service, null, parameters, true, false);
        }

        /// <summary>
        ///     Gets all instances of the specified service using bindings registered with the specified name.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="service">The service to resolve.</param>
        /// <param name="name">The name of the binding.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>A series of instances of the service.</returns>
        public static IEnumerable<object> GetAll(this IResolutionRoot root, Type service, string name, params IParameter[] parameters)
        {
            return GetResolutionIterator(root, service, b => b.Name == name, parameters, true, false);
        }

        /// <summary>
        ///     Gets all instances of the specified service by using the bindings that match the specified constraint.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="service">The service to resolve.</param>
        /// <param name="constraint">The constraint to apply to the bindings.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>A series of instances of the service.</returns>
        public static IEnumerable<object> GetAll(this IResolutionRoot root, Type service, Func<IBindingMetadata, bool> constraint, params IParameter[] parameters)
        {
            return GetResolutionIterator(root, service, constraint, parameters, true, false);
        }

        /// <summary>
        ///     Evaluates if an instance of the specified service can be resolved.
        /// </summary>
        /// <typeparam name="T">The service to resolve.</typeparam>
        /// <param name="root">The resolution root.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>An instance of the service.</returns>
        public static bool CanResolve<T>(this IResolutionRoot root, params IParameter[] parameters)
        {
            return CanResolve(root, typeof (T), null, parameters, false, true);
        }

        /// <summary>
        ///     Evaluates if  an instance of the specified service by using the first binding with the specified name can be resolved.
        /// </summary>
        /// <typeparam name="T">The service to resolve.</typeparam>
        /// <param name="root">The resolution root.</param>
        /// <param name="name">The name of the binding.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>An instance of the service.</returns>
        public static bool CanResolve<T>(this IResolutionRoot root, string name, params IParameter[] parameters)
        {
            return CanResolve(root, typeof (T), b => b.Name == name, parameters, false, true);
        }

        /// <summary>
        ///     Evaluates if  an instance of the specified service by using the first binding that matches the specified constraint can be resolved.
        /// </summary>
        /// <typeparam name="T">The service to resolve.</typeparam>
        /// <param name="root">The resolution root.</param>
        /// <param name="constraint">The constraint to apply to the binding.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>An instance of the service.</returns>
        public static bool CanResolve<T>(this IResolutionRoot root, Func<IBindingMetadata, bool> constraint, params IParameter[] parameters)
        {
            return CanResolve(root, typeof (T), constraint, parameters, false, true);
        }

        /// <summary>
        ///     Gets an instance of the specified service.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="service">The service to resolve.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>An instance of the service.</returns>
        public static object CanResolve(this IResolutionRoot root, Type service, params IParameter[] parameters)
        {
            return CanResolve(root, service, null, parameters, false, true);
        }

        /// <summary>
        ///     Gets an instance of the specified service by using the first binding with the specified name.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="service">The service to resolve.</param>
        /// <param name="name">The name of the binding.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>An instance of the service.</returns>
        public static object CanResolve(this IResolutionRoot root, Type service, string name, params IParameter[] parameters)
        {
            return CanResolve(root, service, b => b.Name == name, parameters, false, true);
        }

        /// <summary>
        ///     Gets an instance of the specified service by using the first binding that matches the specified constraint.
        /// </summary>
        /// <param name="root">The resolution root.</param>
        /// <param name="service">The service to resolve.</param>
        /// <param name="constraint">The constraint to apply to the binding.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        /// <returns>An instance of the service.</returns>
        public static object CanResolve(this IResolutionRoot root, Type service, Func<IBindingMetadata, bool> constraint, params IParameter[] parameters)
        {
            return CanResolve(root, service, constraint, parameters, false, true);
        }

        private static bool CanResolve(IResolutionRoot root, Type service, Func<IBindingMetadata, bool> constraint, IEnumerable<IParameter> parameters, bool isOptional, bool isUnique)
        {
            Ensure.ArgumentNotNull(root, "root");
            Ensure.ArgumentNotNull(service, "service");
            Ensure.ArgumentNotNull(parameters, "parameters");

            var request = root.CreateRequest(service, constraint, parameters, isOptional, isUnique);
            return root.CanResolve(request);
        }

        private static IEnumerable<object> GetResolutionIterator(IResolutionRoot root, Type service, Func<IBindingMetadata, bool> constraint, IEnumerable<IParameter> parameters, bool isOptional, bool isUnique)
        {
            Ensure.ArgumentNotNull(root, "root");
            Ensure.ArgumentNotNull(service, "service");
            Ensure.ArgumentNotNull(parameters, "parameters");

            var request = root.CreateRequest(service, constraint, parameters, isOptional, isUnique);
            return root.Resolve(request);
        }

        private static T TryGet<T>(IEnumerable<T> iterator)
        {
            try
            {
                return iterator.SingleOrDefault();
            }
            catch (ActivationException)
            {
                return default(T);
            }
        }
    }
}