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

using System.Configuration;
using Transformalize.Libs.RazorEngine.Configuration.Xml;

namespace Transformalize.Libs.RazorEngine.Configuration
{
    /// <summary>
    ///     Defines the main configuration section for the RazorEngine.
    /// </summary>
    public class RazorEngineConfigurationSection : ConfigurationSection
    {
        #region Fields

        private const string ActivatorAttribute = "activatorType";
        private const string CompilerServiceFactoryAttribute = "compilerServiceFactoryType";
        private const string DefaultLanguageAttribute = "defaultLanguage";
        private const string NamespacesElement = "namespaces";
        private const string SectionPath = "razorEngine";
        private const string TemplateResolverAttribute = "templateResolverType";
        private const string TemplateServicesElement = "templateServices";

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the activator type.
        /// </summary>
        [ConfigurationProperty(ActivatorAttribute, IsRequired = false)]
        public string ActivatorType
        {
            get { return (string) this[ActivatorAttribute]; }
        }

        /// <summary>
        ///     Gets the compiler service factory type.
        /// </summary>
        [ConfigurationProperty(CompilerServiceFactoryAttribute, IsRequired = false)]
        public string CompilerServiceFactoryType
        {
            get { return (string) this[CompilerServiceFactoryAttribute]; }
        }

        /// <summary>
        ///     Gets or sets the default language.
        /// </summary>
        [ConfigurationProperty(DefaultLanguageAttribute, DefaultValue = Language.CSharp, IsRequired = false)]
        public Language DefaultLanguage
        {
            get { return (Language) this[DefaultLanguageAttribute]; }
            set { this[DefaultLanguageAttribute] = value; }
        }

        /// <summary>
        ///     Gets the collection of namespaces.
        /// </summary>
        [ConfigurationProperty(NamespacesElement, IsRequired = false)]
        public NamespaceConfigurationElementCollection Namespaces
        {
            get { return (NamespaceConfigurationElementCollection) this[NamespacesElement]; }
        }

        /// <summary>
        ///     Gets the template resolver type.
        /// </summary>
        [ConfigurationProperty(TemplateResolverAttribute, IsRequired = false)]
        public string TemplateResolverType
        {
            get { return (string) this[TemplateResolverAttribute]; }
        }

        /// <summary>
        ///     Gets the collection of template service configurations.
        /// </summary>
        [ConfigurationProperty(TemplateServicesElement, IsRequired = false)]
        public TemplateServiceConfigurationElementCollection TemplateServices
        {
            get { return (TemplateServiceConfigurationElementCollection) this[TemplateServicesElement]; }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets an instance of <see cref="RazorEngineConfigurationSection" /> that represents the current configuration.
        /// </summary>
        /// <returns>
        ///     An instance of <see cref="RazorEngineConfigurationSection" />, or null if no configuration is specified.
        /// </returns>
        public static RazorEngineConfigurationSection GetConfiguration()
        {
            return ConfigurationManager.GetSection(SectionPath) as RazorEngineConfigurationSection;
        }

        #endregion
    }
}