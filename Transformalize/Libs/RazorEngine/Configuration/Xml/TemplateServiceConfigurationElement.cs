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

namespace Transformalize.Libs.RazorEngine.Configuration.Xml
{
    /// <summary>
    ///     Defines a configuration of a template service.
    /// </summary>
    public class TemplateServiceConfigurationElement : ConfigurationElement
    {
        #region Fields

        private const string BaseTemplateTypeAttribute = "baseTemplateType";
        private const string CodeInspectorsElement = "codeInspectors";
        private const string DebugAttribute = "debug";
        private const string EncodedStringFactoryAttribute = "encodedStringFactoryType";
        private const string LanguageAttribute = "language";
        private const string NameAttribute = "name";
        private const string NamespacesElement = "namespaces";

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the base template type.
        /// </summary>
        [ConfigurationProperty(BaseTemplateTypeAttribute, IsRequired = false)]
        public string BaseTemplateType
        {
            get { return (string) this[BaseTemplateTypeAttribute]; }
        }

        /// <summary>
        ///     Gets whether the template service is in debug mode.
        /// </summary>
        [ConfigurationProperty(DebugAttribute, IsRequired = false, DefaultValue = false)]
        public bool Debug
        {
            get { return (bool) this[DebugAttribute]; }
        }

        /// <summary>
        ///     Gets the encoded string factory type.
        /// </summary>
        [ConfigurationProperty(EncodedStringFactoryAttribute, IsRequired = false)]
        public string EncodedStringFactoryType
        {
            get { return (string) this[EncodedStringFactoryAttribute]; }
        }

        /// <summary>
        ///     Gets the language.
        /// </summary>
        [ConfigurationProperty(LanguageAttribute, IsRequired = false, DefaultValue = Language.CSharp)]
        public Language Language
        {
            get { return (Language) this[LanguageAttribute]; }
        }

        /// <summary>
        ///     Gets the name of the template service.
        /// </summary>
        [ConfigurationProperty(NameAttribute, IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string) this[NameAttribute]; }
        }

        /// <summary>
        ///     Gets the collection of namespaces.
        /// </summary>
        [ConfigurationProperty(NamespacesElement, IsRequired = false)]
        public NamespaceConfigurationElementCollection Namespaces
        {
            get { return (NamespaceConfigurationElementCollection) this[NamespacesElement]; }
        }

        #endregion
    }
}