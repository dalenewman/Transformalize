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
using Transformalize.Libs.RazorEngine.Compilation;
using Transformalize.Libs.RazorEngine.Compilation.Inspectors;
using Transformalize.Libs.RazorEngine.Templating;
using Transformalize.Libs.RazorEngine.Text;

namespace Transformalize.Libs.RazorEngine.Configuration.Fluent
{
    /// <summary>
    ///     Defines the required contract for implementing a configuration builder.
    /// </summary>
    public interface IConfigurationBuilder
    {
        #region Methods

        /// <summary>
        ///     Sets the activator.
        /// </summary>
        /// <param name="activator">The activator instance.</param>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder ActivateUsing(IActivator activator);

        /// <summary>
        ///     Sets the activator.
        /// </summary>
        /// <typeparam name="TActivator">The activator type.</typeparam>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder ActivateUsing<TActivator>() where TActivator : IActivator, new();

        /// <summary>
        ///     Sets the activator.
        /// </summary>
        /// <param name="activator">The activator delegate.</param>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder ActivateUsing(Func<InstanceContext, ITemplate> activator);

        /// <summary>
        ///     Adds the specified code inspector.
        /// </summary>
        /// <typeparam name="TInspector">The code inspector type.</typeparam>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder AddInspector<TInspector>() where TInspector : ICodeInspector, new();

        /// <summary>
        ///     Adds the specified code inspector.
        /// </summary>
        /// <param name="inspector">The code inspector.</param>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder AddInspector(ICodeInspector inspector);

        /// <summary>
        ///     Sets the compiler service factory.
        /// </summary>
        /// <param name="factory">The compiler service factory.</param>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder CompileUsing(ICompilerServiceFactory factory);

        /// <summary>
        ///     Sets the compiler service factory.
        /// </summary>
        /// <typeparam name="TCompilerServiceFactory">The compiler service factory type.</typeparam>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder CompileUsing<TCompilerServiceFactory>()
            where TCompilerServiceFactory : ICompilerServiceFactory, new();

        /// <summary>
        ///     Sets the encoded string factory.
        /// </summary>
        /// <param name="factory">The encoded string factory.</param>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder EncodeUsing(IEncodedStringFactory factory);

        /// <summary>
        ///     Sets the encoded string factory.
        /// </summary>
        /// <typeparam name="TEncodedStringFactory">The encoded string factory type.</typeparam>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder EncodeUsing<TEncodedStringFactory>() where TEncodedStringFactory : IEncodedStringFactory, new();

        /// <summary>
        ///     Sets the resolve used to locate unknown templates.
        /// </summary>
        /// <typeparam name="TResolver">The resolve type.</typeparam>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder ResolveUsing<TResolver>() where TResolver : ITemplateResolver, new();

        /// <summary>
        ///     Sets the resolver used to locate unknown templates.
        /// </summary>
        /// <param name="resolver">The resolver instance to use.</param>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder ResolveUsing(ITemplateResolver resolver);

        /// <summary>
        ///     Sets the resolver delegate used to locate unknown templates.
        /// </summary>
        /// <param name="resolver">The resolver delegate to use.</param>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder ResolveUsing(Func<string, string> resolver);

        /// <summary>
        ///     Includes the specified namespaces
        /// </summary>
        /// <param name="namespaces">The set of namespaces to include.</param>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder IncludeNamespaces(params string[] namespaces);

        /// <summary>
        ///     Sets the default activator.
        /// </summary>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder UseDefaultActivator();

        /// <summary>
        ///     Sets the default compiler service factory.
        /// </summary>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder UseDefaultCompilerServiceFactory();

        /// <summary>
        ///     Sets the default encoded string factory.
        /// </summary>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder UseDefaultEncodedStringFactory();

        /// <summary>
        ///     Sets the base template type.
        /// </summary>
        /// <param name="baseTemplateType">The base template type.</param>
        /// <returns>The current configuration builder/.</returns>
        IConfigurationBuilder WithBaseTemplateType(Type baseTemplateType);

        /// <summary>
        ///     Sets the code language.
        /// </summary>
        /// <param name="language">The code language.</param>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder WithCodeLanguage(Language language);

        /// <summary>
        ///     Sets the encoding.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        /// <returns>The current configuration builder.</returns>
        IConfigurationBuilder WithEncoding(Encoding encoding);

        #endregion
    }
}