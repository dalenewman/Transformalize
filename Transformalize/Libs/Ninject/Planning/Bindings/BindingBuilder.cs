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
using Transformalize.Libs.Ninject.Activation.Providers;
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Parameters;
using Transformalize.Libs.Ninject.Syntax;

namespace Transformalize.Libs.Ninject.Planning.Bindings
{
#if !NETCF
#endif

    /// <summary>
    ///     Provides a root for the fluent syntax associated with an <see cref="BindingConfiguration" />.
    /// </summary>
    public class BindingBuilder
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BindingBuilder" /> class.
        /// </summary>
        /// <param name="bindingConfiguration">The binding to build.</param>
        /// <param name="kernel">The kernel.</param>
        /// <param name="serviceNames">The names of the services.</param>
        public BindingBuilder(IBindingConfiguration bindingConfiguration, IKernel kernel, string serviceNames)
        {
            Ensure.ArgumentNotNull(bindingConfiguration, "binding");
            Ensure.ArgumentNotNull(kernel, "kernel");
            BindingConfiguration = bindingConfiguration;
            Kernel = kernel;
            ServiceNames = serviceNames;
            BindingConfiguration.ScopeCallback = kernel.Settings.DefaultScopeCallback;
        }

        /// <summary>
        ///     Gets the binding being built.
        /// </summary>
        public IBindingConfiguration BindingConfiguration { get; private set; }

        /// <summary>
        ///     Gets the kernel.
        /// </summary>
        public IKernel Kernel { get; private set; }

        /// <summary>
        ///     Gets the names of the services.
        /// </summary>
        /// <value>The names of the services.</value>
        protected string ServiceNames { get; private set; }

        /// <summary>
        ///     Indicates that the service should be bound to the specified implementation type.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <returns>The fluent syntax.</returns>
        protected IBindingWhenInNamedWithOrOnSyntax<TImplementation> InternalTo<TImplementation>()
        {
            return InternalTo<TImplementation>(typeof (TImplementation));
        }

        /// <summary>
        ///     Indicates that the service should be bound to the specified implementation type.
        /// </summary>
        /// <typeparam name="T">The type of the returned syntax.</typeparam>
        /// <param name="implementation">The implementation type.</param>
        /// <returns>The fluent syntax.</returns>
        protected IBindingWhenInNamedWithOrOnSyntax<T> InternalTo<T>(Type implementation)
        {
            BindingConfiguration.ProviderCallback = StandardProvider.GetCreationCallback(implementation);
            BindingConfiguration.Target = BindingTarget.Type;

            return new BindingConfigurationBuilder<T>(BindingConfiguration, ServiceNames, Kernel);
        }

        /// <summary>
        ///     Indicates that the service should be bound to the specified constant value.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="value">The constant value.</param>
        /// <returns>The fluent syntax.</returns>
        protected IBindingWhenInNamedWithOrOnSyntax<TImplementation> InternalToConfiguration<TImplementation>(TImplementation value)
        {
            BindingConfiguration.ProviderCallback = ctx => new ConstantProvider<TImplementation>(value);
            BindingConfiguration.Target = BindingTarget.Constant;
            BindingConfiguration.ScopeCallback = StandardScopeCallbacks.Singleton;

            return new BindingConfigurationBuilder<TImplementation>(BindingConfiguration, ServiceNames, Kernel);
        }

        /// <summary>
        ///     Indicates that the service should be bound to the specified callback method.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="method">The method.</param>
        /// <returns>The fluent syntax.</returns>
        protected IBindingWhenInNamedWithOrOnSyntax<TImplementation> InternalToMethod<TImplementation>(Func<IContext, TImplementation> method)
        {
            BindingConfiguration.ProviderCallback = ctx => new CallbackProvider<TImplementation>(method);
            BindingConfiguration.Target = BindingTarget.Method;

            return new BindingConfigurationBuilder<TImplementation>(BindingConfiguration, ServiceNames, Kernel);
        }

        /// <summary>
        ///     Indicates that the service should be bound to the specified provider.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="provider">The provider.</param>
        /// <returns>The fluent syntax.</returns>
        protected IBindingWhenInNamedWithOrOnSyntax<TImplementation> InternalToProvider<TImplementation>(IProvider<TImplementation> provider)
        {
            BindingConfiguration.ProviderCallback = ctx => provider;
            BindingConfiguration.Target = BindingTarget.Provider;

            return new BindingConfigurationBuilder<TImplementation>(BindingConfiguration, ServiceNames, Kernel);
        }

        /// <summary>
        ///     Indicates that the service should be bound to an instance of the specified provider type.
        ///     The instance will be activated via the kernel when an instance of the service is activated.
        /// </summary>
        /// <typeparam name="TProvider">The type of provider to activate.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <returns>The fluent syntax.</returns>
        protected IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToProviderInternal<TProvider, TImplementation>()
            where TProvider : IProvider
        {
            BindingConfiguration.ProviderCallback = ctx => ctx.Kernel.Get<TProvider>();
            BindingConfiguration.Target = BindingTarget.Provider;

            return new BindingConfigurationBuilder<TImplementation>(BindingConfiguration, ServiceNames, Kernel);
        }

        /// <summary>
        ///     Indicates that the service should be bound to an instance of the specified provider type.
        ///     The instance will be activated via the kernel when an instance of the service is activated.
        /// </summary>
        /// <typeparam name="T">The type of the returned fleunt syntax</typeparam>
        /// <param name="providerType">The type of provider to activate.</param>
        /// <returns>The fluent syntax.</returns>
        protected IBindingWhenInNamedWithOrOnSyntax<T> ToProviderInternal<T>(Type providerType)
        {
            BindingConfiguration.ProviderCallback = ctx => ctx.Kernel.Get(providerType) as IProvider;
            BindingConfiguration.Target = BindingTarget.Provider;

            return new BindingConfigurationBuilder<T>(BindingConfiguration, ServiceNames, Kernel);
        }

#if !NETCF
        /// <summary>
        ///     Indicates that the service should be bound to the speecified constructor.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="newExpression">The expression that specifies the constructor.</param>
        /// <returns>The fluent syntax.</returns>
        protected IBindingWhenInNamedWithOrOnSyntax<TImplementation> InternalToConstructor<TImplementation>(
            Expression<Func<IConstructorArgumentSyntax, TImplementation>> newExpression)
        {
            var ctorExpression = newExpression.Body as NewExpression;
            if (ctorExpression == null)
            {
                throw new ArgumentException("The expression must be a constructor call.", "newExpression");
            }

            BindingConfiguration.ProviderCallback = StandardProvider.GetCreationCallback(ctorExpression.Type, ctorExpression.Constructor);
            BindingConfiguration.Target = BindingTarget.Type;
            AddConstructorArguments(ctorExpression, newExpression.Parameters[0]);

            return new BindingConfigurationBuilder<TImplementation>(BindingConfiguration, ServiceNames, Kernel);
        }

        /// <summary>
        ///     Adds the constructor arguments for the specified constructor expression.
        /// </summary>
        /// <param name="ctorExpression">The ctor expression.</param>
        /// <param name="constructorArgumentSyntaxParameterExpression">The constructor argument syntax parameter expression.</param>
        private void AddConstructorArguments(NewExpression ctorExpression, ParameterExpression constructorArgumentSyntaxParameterExpression)
        {
            var parameters = ctorExpression.Constructor.GetParameters();

            for (var i = 0; i < ctorExpression.Arguments.Count; i++)
            {
                var argument = ctorExpression.Arguments[i];
                var argumentName = parameters[i].Name;

                AddConstructorArgument(argument, argumentName, constructorArgumentSyntaxParameterExpression);
            }
        }

        /// <summary>
        ///     Adds a constructor argument for the specified argument expression.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="argumentName">Name of the argument.</param>
        /// <param name="constructorArgumentSyntaxParameterExpression">The constructor argument syntax parameter expression.</param>
        private void AddConstructorArgument(Expression argument, string argumentName, ParameterExpression constructorArgumentSyntaxParameterExpression)
        {
            var methodCall = argument as MethodCallExpression;
            if (methodCall == null ||
                !methodCall.Method.IsGenericMethod ||
                methodCall.Method.GetGenericMethodDefinition().DeclaringType != typeof (IConstructorArgumentSyntax))
            {
                var compiledExpression = Expression.Lambda(argument, constructorArgumentSyntaxParameterExpression).Compile();
                BindingConfiguration.Parameters.Add(new ConstructorArgument(
                                                        argumentName,
                                                        ctx => compiledExpression.DynamicInvoke(new ConstructorArgumentSyntax(ctx))));
            }
        }

        /// <summary>
        ///     Passed to ToConstructor to specify that a constructor value is Injected.
        /// </summary>
        private class ConstructorArgumentSyntax : IConstructorArgumentSyntax
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="ConstructorArgumentSyntax" /> class.
            /// </summary>
            /// <param name="context">The context.</param>
            public ConstructorArgumentSyntax(IContext context)
            {
                Context = context;
            }

            /// <summary>
            ///     Gets the context.
            /// </summary>
            /// <value>The context.</value>
            public IContext Context { get; private set; }

            /// <summary>
            ///     Specifies that the argument is injected.
            /// </summary>
            /// <typeparam name="T1">The type of the parameter</typeparam>
            /// <returns>Not used. This interface has no implementation.</returns>
            public T1 Inject<T1>()
            {
                throw new InvalidOperationException("This method is for declaration that a parameter shall be injected only! Never call it directly.");
            }
        }
#endif
    }
}