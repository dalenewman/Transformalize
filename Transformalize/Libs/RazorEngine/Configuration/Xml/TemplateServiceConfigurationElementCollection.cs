#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Configuration;

namespace Transformalize.Libs.RazorEngine.Configuration.Xml
{
    /// <summary>
    ///     Defines a collection of <see cref="TemplateServiceConfigurationElement" /> instances.
    /// </summary>
    [ConfigurationCollection(typeof (TemplateServiceConfigurationElement), AddItemName = "service")]
    public class TemplateServiceConfigurationElementCollection : ConfigurationElementCollection
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
            return new TemplateServiceConfigurationElement();
        }

        /// <summary>
        ///     Gets a unique key for the specified element.
        /// </summary>
        /// <param name="element">The configuration element.</param>
        /// <returns>The key for the element.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TemplateServiceConfigurationElement) element).Name;
        }

        #endregion
    }
}