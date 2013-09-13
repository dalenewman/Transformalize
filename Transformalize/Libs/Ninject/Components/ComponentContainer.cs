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
using System.Reflection;
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Infrastructure.Disposal;
using Transformalize.Libs.Ninject.Infrastructure.Introspection;
using Transformalize.Libs.Ninject.Infrastructure.Language;

#endregion

namespace Transformalize.Libs.Ninject.Components
{
    /// <summary>
    ///     An internal container that manages and resolves components that contribute to Ninject.
    /// </summary>
    public class ComponentContainer : DisposableObject, IComponentContainer
    {
        private readonly Multimap<Type, Type> _mappings = new Multimap<Type, Type>();
        private readonly Dictionary<Type, INinjectComponent> _instances = new Dictionary<Type, INinjectComponent>();
        private readonly HashSet<KeyValuePair<Type, Type>> transients = new HashSet<KeyValuePair<Type, Type>>();

        /// <summary>
        ///     Gets or sets the kernel that owns the component container.
        /// </summary>
        public IKernel Kernel { get; set; }

        /// <summary>
        ///     Releases resources held by the object.
        /// </summary>
        public override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                foreach (var instance in _instances.Values)
                    instance.Dispose();

                _mappings.Clear();
                _instances.Clear();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        ///     Registers a component in the container.
        /// </summary>
        /// <typeparam name="TComponent">The component type.</typeparam>
        /// <typeparam name="TImplementation">The component's implementation type.</typeparam>
        public void Add<TComponent, TImplementation>()
            where TComponent : INinjectComponent
            where TImplementation : TComponent, INinjectComponent
        {
            _mappings.Add(typeof (TComponent), typeof (TImplementation));
        }

        /// <summary>
        ///     Registers a transient component in the container.
        /// </summary>
        /// <typeparam name="TComponent">The component type.</typeparam>
        /// <typeparam name="TImplementation">The component's implementation type.</typeparam>
        public void AddTransient<TComponent, TImplementation>()
            where TComponent : INinjectComponent
            where TImplementation : TComponent, INinjectComponent
        {
            Add<TComponent, TImplementation>();
            transients.Add(new KeyValuePair<Type, Type>(typeof (TComponent), typeof (TImplementation)));
        }

        /// <summary>
        ///     Removes all registrations for the specified component.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        public void RemoveAll<T>()
            where T : INinjectComponent
        {
            RemoveAll(typeof (T));
        }

        /// <summary>
        ///     Removes all registrations for the specified component.
        /// </summary>
        /// <param name="component">The component type.</param>
        public void RemoveAll(Type component)
        {
            Ensure.ArgumentNotNull(component, "component");

            foreach (var implementation in _mappings[component])
            {
                if (_instances.ContainsKey(implementation))
                    _instances[implementation].Dispose();

                _instances.Remove(implementation);
            }

            _mappings.RemoveAll(component);
        }

        /// <summary>
        ///     Gets one instance of the specified component.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>The instance of the component.</returns>
        public T Get<T>()
            where T : INinjectComponent
        {
            return (T) Get(typeof (T));
        }

        /// <summary>
        ///     Gets all available instances of the specified component.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>A series of instances of the specified component.</returns>
        public IEnumerable<T> GetAll<T>()
            where T : INinjectComponent
        {
            return GetAll(typeof (T)).Cast<T>();
        }

        /// <summary>
        ///     Gets one instance of the specified component.
        /// </summary>
        /// <param name="component">The component type.</param>
        /// <returns>The instance of the component.</returns>
        public object Get(Type component)
        {
            Ensure.ArgumentNotNull(component, "component");

            if (component == typeof (IKernel))
                return Kernel;

            if (component.IsGenericType)
            {
                var gtd = component.GetGenericTypeDefinition();
                var argument = component.GetGenericArguments()[0];

#if WINDOWS_PHONE
                Type discreteGenericType =
                    typeof (IEnumerable<>).MakeGenericType(argument);
                if (gtd.IsInterface && discreteGenericType.IsAssignableFrom(component))
                    return GetAll(argument).CastSlow(argument);
#else
                if (gtd.IsInterface && typeof (IEnumerable<>).IsAssignableFrom(gtd))
                    return GetAll(argument).CastSlow(argument);
#endif
            }
            var implementation = _mappings[component].FirstOrDefault();

            if (implementation == null)
                throw new InvalidOperationException(ExceptionFormatter.NoSuchComponentRegistered(component));

            return ResolveInstance(component, implementation);
        }

        /// <summary>
        ///     Gets all available instances of the specified component.
        /// </summary>
        /// <param name="component">The component type.</param>
        /// <returns>A series of instances of the specified component.</returns>
        public IEnumerable<object> GetAll(Type component)
        {
            Ensure.ArgumentNotNull(component, "component");

            return _mappings[component]
                .Select(implementation => ResolveInstance(component, implementation));
        }

        private object ResolveInstance(Type component, Type implementation)
        {
            lock (_instances)
                return _instances.ContainsKey(implementation) ? _instances[implementation] : CreateNewInstance(component, implementation);
        }

        private object CreateNewInstance(Type component, Type implementation)
        {
            var constructor = SelectConstructor(component, implementation);
            var arguments = constructor.GetParameters().Select(parameter => Get(parameter.ParameterType)).ToArray();

            try
            {
                var instance = constructor.Invoke(arguments) as INinjectComponent;
                instance.Settings = Kernel.Settings;

                if (!transients.Contains(new KeyValuePair<Type, Type>(component, implementation)))
                {
                    _instances.Add(implementation, instance);
                }

                return instance;
            }
            catch (TargetInvocationException ex)
            {
                ex.RethrowInnerException();
                return null;
            }
        }

        private static ConstructorInfo SelectConstructor(Type component, Type implementation)
        {
            var constructor = implementation.GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();

            if (constructor == null)
                throw new InvalidOperationException(ExceptionFormatter.NoConstructorsAvailableForComponent(component, implementation));

            return constructor;
        }

#if SILVERLIGHT_30 || SILVERLIGHT_20 || WINDOWS_PHONE || NETCF_35
        private class HashSet<T>
        {
            private IDictionary<T, object> data = new Dictionary<T,object>();
 
            public void Add(T o)
            {
                this.data.Add(o, null);
            }

            public bool Contains(T o)
            {
                return this.data.ContainsKey(o);
            }
        }
#endif
    }
}