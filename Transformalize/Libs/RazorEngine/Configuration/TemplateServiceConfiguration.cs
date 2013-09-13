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
using Transformalize.Libs.RazorEngine.Compilation;
using Transformalize.Libs.RazorEngine.Compilation.Inspectors;
using Transformalize.Libs.RazorEngine.Templating;
using Transformalize.Libs.RazorEngine.Text;

namespace Transformalize.Libs.RazorEngine.Configuration
{
    /// <summary>
    ///     Provides a default implementation of a template service configuration.
    /// </summary>
    public class TemplateServiceConfiguration : ITemplateServiceConfiguration
    {
        #region Constructor

        /// <summary>
        ///     Initialises a new instance of <see cref="TemplateServiceConfiguration" />.
        /// </summary>
        public TemplateServiceConfiguration()
        {
            Activator = new DefaultActivator();
            CompilerServiceFactory = new DefaultCompilerServiceFactory();
            EncodedStringFactory = new HtmlEncodedStringFactory();
            CodeInspectors = new List<ICodeInspector>();

            Namespaces = new HashSet<string>
                             {
                                 "System",
                                 "System.Collections.Generic",
                                 "System.Linq"
                             };

            var config = RazorEngineConfigurationSection.GetConfiguration();
            Language = (config == null)
                           ? Language.CSharp
                           : config.DefaultLanguage;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the set of code inspectors.
        /// </summary>
        public IList<ICodeInspector> CodeInspectors { get; private set; }

        /// <summary>
        ///     Gets or sets the activator.
        /// </summary>
        public IActivator Activator { get; set; }

        /// <summary>
        ///     Gets or sets the base template type.
        /// </summary>
        public Type BaseTemplateType { get; set; }

        /// <summary>
        ///     Gets the set of code inspectors.
        /// </summary>
        IEnumerable<ICodeInspector> ITemplateServiceConfiguration.CodeInspectors
        {
            get { return CodeInspectors; }
        }

        /// <summary>
        ///     Gets or sets the compiler service factory.
        /// </summary>
        public ICompilerServiceFactory CompilerServiceFactory { get; set; }

        /// <summary>
        ///     Gets whether the template service is operating in debug mode.
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        ///     Gets or sets the encoded string factory.
        /// </summary>
        public IEncodedStringFactory EncodedStringFactory { get; set; }

        /// <summary>
        ///     Gets or sets the language.
        /// </summary>
        public Language Language { get; set; }

        /// <summary>
        ///     Gets or sets the collection of namespaces.
        /// </summary>
        public ISet<string> Namespaces { get; set; }

        /// <summary>
        ///     Gets or sets the template resolver.
        /// </summary>
        public ITemplateResolver Resolver { get; set; }

        #endregion
    }
}