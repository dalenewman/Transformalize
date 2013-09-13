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
using System.Linq.Expressions;
using Transformalize.Libs.Ninject.Activation;
using Transformalize.Libs.Ninject.Syntax;

namespace Transformalize.Libs.Ninject.Planning.Bindings
{
#if !NETCF
#endif

    /// <summary>
    ///     Provides a root for the fluent syntax associated with an <see cref="BindingBuilder.BindingConfiguration" />.
    /// </summary>
    /// <typeparam name="T1">The first service type.</typeparam>
    /// <typeparam name="T2">The second service type.</typeparam>
    /// <typeparam name="T3">The third service type.</typeparam>
    /// <typeparam name="T4">The fourth service type.</typeparam>
    public class BindingBuilder<T1, T2, T3, T4> : BindingBuilder, IBindingToSyntax<T1, T2, T3, T4>
    {
#pragma warning disable 1584 //mono compiler bug
        /// <summary>
        ///     Initializes a new instance of the <see cref="BindingBuilder{T1, T2, T3, T4}" /> class.
        /// </summary>
        /// <param name="bindingConfigurationConfiguration">The binding to build.</param>
        /// <param name="kernel">The kernel.</param>
        /// <param name="serviceNames">The names of the services.</param>
        public BindingBuilder(IBindingConfiguration bindingConfigurationConfiguration, IKernel kernel, string serviceNames)
            : base(bindingConfigurationConfiguration, kernel, serviceNames)
        {
        }
#pragma warning restore 1584

        /// <summary>
        ///     Indicates that the service should be bound to the specified implementation type.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> To<TImplementation>()
            where TImplementation : T1, T2, T3, T4
        {
            return InternalTo<TImplementation>();
        }

        /// <summary>
        ///     Indicates that the service should be bound to the specified implementation type.
        /// </summary>
        /// <param name="implementation">The implementation type.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<object> To(Type implementation)
        {
            return InternalTo<object>(implementation);
        }

#if !NETCF
        /// <summary>
        ///     Indicates that the service should be bound to the speecified constructor.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="newExpression">The expression that specifies the constructor.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToConstructor<TImplementation>(
            Expression<Func<IConstructorArgumentSyntax, TImplementation>> newExpression)
            where TImplementation : T1, T2, T3, T4
        {
            return InternalToConstructor(newExpression);
        }
#endif

        /// <summary>
        ///     Indicates that the service should be bound to an instance of the specified provider type.
        ///     The instance will be activated via the kernel when an instance of the service is activated.
        /// </summary>
        /// <typeparam name="TProvider">The type of provider to activate.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<object> ToProvider<TProvider>()
            where TProvider : IProvider
        {
            return ToProviderInternal<TProvider, object>();
        }

        /// <summary>
        ///     Indicates that the service should be bound to an instance of the specified provider type.
        ///     The instance will be activated via the kernel when an instance of the service is activated.
        /// </summary>
        /// <typeparam name="TProvider">The type of provider to activate.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToProvider<TProvider, TImplementation>()
            where TProvider : IProvider<TImplementation>
            where TImplementation : T1, T2, T3, T4
        {
            return ToProviderInternal<TProvider, TImplementation>();
        }

        /// <summary>
        ///     Indicates that the service should be bound to an instance of the specified provider type.
        ///     The instance will be activated via the kernel when an instance of the service is activated.
        /// </summary>
        /// <param name="providerType">The type of provider to activate.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<object> ToProvider(Type providerType)
        {
            return ToProviderInternal<object>(providerType);
        }

        /// <summary>
        ///     Indicates that the service should be bound to the specified provider.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="provider">The provider.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToProvider<TImplementation>(IProvider<TImplementation> provider)
            where TImplementation : T1, T2, T3, T4
        {
            return InternalToProvider(provider);
        }

        /// <summary>
        ///     Indicates that the service should be bound to the specified callback method.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="method">The method.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToMethod<TImplementation>(Func<IContext, TImplementation> method)
            where TImplementation : T1, T2, T3
        {
            return InternalToMethod(method);
        }

        /// <summary>
        ///     Indicates that the service should be bound to the specified constant value.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="value">The constant value.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToConstant<TImplementation>(TImplementation value)
            where TImplementation : T1, T2, T3, T4
        {
            return InternalToConfiguration(value);
        }
    }
}