using System.Configuration;

namespace Transformalize.Libs.RazorEngine.Configuration.Xml
{
    /// <summary>
    ///     Defines a configuration of a namespace.
    /// </summary>
    public class NamespaceConfigurationElement : ConfigurationElement
    {
        #region Fields

        private const string NamespaceAttribute = "namespace";

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the namespace.
        /// </summary>
        [ConfigurationProperty(NamespaceAttribute, IsRequired = true)]
        public string Namespace
        {
            get { return (string) this[NamespaceAttribute]; }
        }

        #endregion
    }
}