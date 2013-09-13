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
using System.Configuration;
using System.Linq;
using Transformalize.Libs.RazorEngine.Compilation;
using Transformalize.Libs.RazorEngine.Compilation.Inspectors;
using Transformalize.Libs.RazorEngine.Templating;
using Transformalize.Libs.RazorEngine.Text;

namespace Transformalize.Libs.RazorEngine.Configuration.Xml
{
    /// <summary>
    ///     Represents a template service configuration that supports the xml configuration mechanism.
    /// </summary>
    public class XmlTemplateServiceConfiguration : ITemplateServiceConfiguration
    {
        #region Constructor

        /// <summary>
        ///     Initialises a new instance of <see cref="XmlTemplateServiceConfiguration" />.
        /// </summary>
        /// <param name="name">The name of the template service configuration.</param>
        public XmlTemplateServiceConfiguration(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("'name' is a required parameter.", "name");

            Namespaces = new HashSet<string>();

            InitialiseConfiguration(name);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the activator.
        /// </summary>
        public IActivator Activator { get; private set; }

        /// <summary>
        ///     Gets the base template type.
        /// </summary>
        public Type BaseTemplateType { get; private set; }

        /// <summary>
        ///     Gets the code inspectors.
        /// </summary>
        public IEnumerable<ICodeInspector> CodeInspectors { get; private set; }

        /// <summary>
        ///     Gets the compiler service factory.
        /// </summary>
        public ICompilerServiceFactory CompilerServiceFactory { get; private set; }

        /// <summary>
        ///     Gets whether the template service is operating in debug mode.
        /// </summary>
        public bool Debug { get; private set; }

        /// <summary>
        ///     Gets the encoded string factory.
        /// </summary>
        public IEncodedStringFactory EncodedStringFactory { get; private set; }

        /// <summary>
        ///     Gets the language.
        /// </summary>
        public Language Language { get; private set; }

        /// <summary>
        ///     Gets the namespaces.
        /// </summary>
        public ISet<string> Namespaces { get; private set; }

        /// <summary>
        ///     Gets the template resolver.
        /// </summary>
        public ITemplateResolver Resolver { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Adds the namespaces from the specified collection.
        /// </summary>
        /// <param name="namespaces">The set of namespace configurations.</param>
        private void AddNamespaces(NamespaceConfigurationElementCollection namespaces)
        {
            if (namespaces == null || namespaces.Count == 0)
                return;

            foreach (NamespaceConfigurationElement config in namespaces)
                Namespaces.Add(config.Namespace);
        }

        /// <summary>
        ///     Gets an instance of the specified type.
        /// </summary>
        /// <typeparam name="T">The expected instance type.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>The instance.</returns>
        private T GetInstance<T>(Type type)
        {
            var instanceType = typeof (T);

            if (!instanceType.IsAssignableFrom(type))
                throw new ConfigurationErrorsException("The type '" + type.FullName + "' is not assignable to type '" + instanceType.FullName + "'");

            return (T) System.Activator.CreateInstance(type);
        }

        /// <summary>
        ///     Gets the type with the specified name.
        /// </summary>
        /// <param name="typeName">The type name.</param>
        /// <returns></returns>
        private Type GetType(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                return null;

            var type = Type.GetType(typeName);
            if (type == null)
                throw new ConfigurationErrorsException("The type '" + typeName + "' could not be loaded.");

            return type;
        }

        /// <summary>
        ///     Initialises the configuration.
        /// </summary>
        /// <param name="name">The name of the template service configuration.</param>
        private void InitialiseConfiguration(string name)
        {
            var config = RazorEngineConfigurationSection.GetConfiguration();
            if (config == null)
                throw new ConfigurationErrorsException("No <razorEngine> configuration section has been defined.");

            var serviceConfig = config.TemplateServices
                                      .OfType<TemplateServiceConfigurationElement>()
                                      .Where(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                                      .SingleOrDefault();

            if (serviceConfig == null)
                throw new ConfigurationErrorsException("No <templateService> configuration element defined with name = '" + name + "'");

            InitialiseConfiguration(config, serviceConfig);
        }

        /// <summary>
        ///     Initialises the configuration.
        /// </summary>
        /// <param name="config">The core configuration.</param>
        /// <param name="serviceConfig">The service configuration.</param>
        private void InitialiseConfiguration(RazorEngineConfigurationSection config, TemplateServiceConfigurationElement serviceConfig)
        {
            // Add the global namespaces.
            AddNamespaces(config.Namespaces);

            // Add the specific namespaces.
            AddNamespaces(serviceConfig.Namespaces);

            // Sets the activator.
            SetActivator(config.ActivatorType);

            // Sets the base template type.
            SetBaseTemplateType(serviceConfig.BaseTemplateType);

            // Sets the compiler service factory.
            SetCompilerServiceFactory(config.CompilerServiceFactoryType);

            Debug = serviceConfig.Debug;

            // Sets the encoded string factory.
            SetEncodedStringFactory(serviceConfig.EncodedStringFactoryType);

            Language = serviceConfig.Language;

            // Sets the tempalte resolver.
            SetTemplateResolver(config.TemplateResolverType);
        }

        /// <summary>
        ///     Sets the activator.
        /// </summary>
        /// <param name="activatorType">The activator type.</param>
        private void SetActivator(string activatorType)
        {
            var type = GetType(activatorType);
            if (type != null)
                Activator = GetInstance<IActivator>(type);
        }

        /// <summary>
        ///     Sets the base template type.
        /// </summary>
        /// <param name="baseTemplateType">The base template type.</param>
        private void SetBaseTemplateType(string baseTemplateType)
        {
            var type = GetType(baseTemplateType);
            if (type != null)
                BaseTemplateType = type;
        }

        /// <summary>
        ///     Sets the compiler service factory.
        /// </summary>
        /// <param name="compilerServiceFactoryType">The compiler service factory type.</param>
        private void SetCompilerServiceFactory(string compilerServiceFactoryType)
        {
            var type = GetType(compilerServiceFactoryType);
            if (type != null)
                CompilerServiceFactory = GetInstance<ICompilerServiceFactory>(type);
        }

        /// <summary>
        ///     Sets the encoded string factory.
        /// </summary>
        /// <param name="encodedStringFactoryType"></param>
        private void SetEncodedStringFactory(string encodedStringFactoryType)
        {
            var type = GetType(encodedStringFactoryType);
            if (type != null)
                EncodedStringFactory = GetInstance<IEncodedStringFactory>(type);
        }

        /// <summary>
        ///     Sets the template resolver.
        /// </summary>
        /// <param name="templateResolverType">The template resolver type.</param>
        private void SetTemplateResolver(string templateResolverType)
        {
            var type = GetType(templateResolverType);
            if (type != null)
                Resolver = GetInstance<ITemplateResolver>(type);
        }

        #endregion
    }
}