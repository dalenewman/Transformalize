#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Configuration;

namespace Transformalize.Libs.RazorEngine.Configuration.Xml
{
    /// <summary>
    ///     Defines a collection of <see cref="NamespaceConfigurationElement" /> instances.
    /// </summary>
    [ConfigurationCollection(typeof (TemplateServiceConfigurationElement))]
    public class NamespaceConfigurationElementCollection : ConfigurationElementCollection
    {
        #region Methods

        /// <summary>
        ///     Creates a new <see cref="ConfigurationElement" /> for use with the collection.
        /// </summary>
        /// <returns>
        ///     The <see cref="ConfigurationElement" /> instance.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new NamespaceConfigurationElement();
        }

        /// <summary>
        ///     Gets a unique key for the specified element.
        /// </summary>
        /// <param name="element">The configuration element.</param>
        /// <returns>The key for the element.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((NamespaceConfigurationElement) element).Namespace;
        }

        #endregion
    }
}