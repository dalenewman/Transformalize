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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Transformalize.Libs.RazorEngine.Compilation;
using Transformalize.Libs.RazorEngine.Compilation.Inspectors;
using Transformalize.Libs.RazorEngine.Templating;
using Transformalize.Libs.RazorEngine.Text;

namespace Transformalize.Libs.RazorEngine.Configuration.Fluent
{
    /// <summary>
    ///     Defines a fluent template service configuration
    /// </summary>
    public class FluentTemplateServiceConfiguration : ITemplateServiceConfiguration
    {
        #region Fields

        private readonly TemplateServiceConfiguration _innerConfig = new TemplateServiceConfiguration();

        #endregion

        #region Constructor

        /// <summary>
        ///     Initialises a new instance of <see cref="FluentTemplateServiceConfiguration" />.
        /// </summary>
        /// <param name="config">The delegate used to create the configuration.</param>
        public FluentTemplateServiceConfiguration(Action<IConfigurationBuilder> config)
        {
            Contract.Requires(config != null);

            config(new FluentConfigurationBuilder(_innerConfig));
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the activator.
        /// </summary>
        public IActivator Activator
        {
            get { return _innerConfig.Activator; }
        }

        /// <summary>
        ///     Gets the base template type.
        /// </summary>
        public Type BaseTemplateType
        {
            get { return _innerConfig.BaseTemplateType; }
        }

        /// <summary>
        ///     Gets the set of code inspectors.
        /// </summary>
        public IEnumerable<ICodeInspector> CodeInspectors
        {
            get { return _innerConfig.CodeInspectors; }
        }

        /// <summary>
        ///     Gets or sets the compiler service factory.
        /// </summary>
        public ICompilerServiceFactory CompilerServiceFactory
        {
            get { return _innerConfig.CompilerServiceFactory; }
        }

        /// <summary>
        ///     Gets whether the template service is operating in debug mode.
        /// </summary>
        public bool Debug
        {
            get { return _innerConfig.Debug; }
        }

        /// <summary>
        ///     Gets or sets the encoded string factory.
        /// </summary>
        public IEncodedStringFactory EncodedStringFactory
        {
            get { return _innerConfig.EncodedStringFactory; }
        }

        /// <summary>
        ///     Gets or sets the language.
        /// </summary>
        public Language Language
        {
            get { return _innerConfig.Language; }
        }

        /// <summary>
        ///     Gets or sets the collection of namespaces.
        /// </summary>
        public ISet<string> Namespaces
        {
            get { return _innerConfig.Namespaces; }
        }

        /// <summary>
        ///     Gets the resolver.
        /// </summary>
        public ITemplateResolver Resolver
        {
            get { return _innerConfig.Resolver; }
        }

        #endregion
    }
}