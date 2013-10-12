//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Core
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design.Validation;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
    /// <summary>
    /// Configuration section for the configuration sources.
    /// </summary>
	/// <remarks>
	/// This configuration must reside in the application's default configuration file.
	/// </remarks>
    [ViewModel(ConfigurationSourcesDesignTime.ViewModelTypeNames.ConfigurationSourceSectionViewModel)]
    [ResourceDescription(typeof(DesignResources), "ConfigurationSourceSectionDescription")]
    [ResourceDisplayName(typeof(DesignResources), "ConfigurationSourceSectionDisplayName")]
    [EnvironmentalOverrides(false)]
    public class ConfigurationSourceSection : SerializableConfigurationSection
    {
        private const string selectedSourceProperty = "selectedSource";
        private const string sourcesProperty = "sources";
        private const string parentSourceProperty = "parentSource";
        private const string redirectSectionsProperty = "redirectSections";

        /// <summary>
		/// This field supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
		/// </summary>
        public const string SectionName = "enterpriseLibrary.ConfigurationSource";        

        /// <summary>
		/// Returns the <see cref="ConfigurationSourceSection"/> from the application's default configuration file.
        /// </summary>
		/// <returns>The section from the configuration file, or <see langword="null"/> (<b>Nothing</b> in Visual Basic) if the section is not present in the configuration file.</returns>
        public static ConfigurationSourceSection GetConfigurationSourceSection()
        {
			return (ConfigurationSourceSection)ConfigurationManager.GetSection(SectionName);
        }

        /// <summary>
        /// Gets or sets the name for the default configuration source.
        /// </summary>
        [ConfigurationProperty(selectedSourceProperty, IsRequired=true)]
        [ResourceDescription(typeof(DesignResources), "ConfigurationSourceSectionSelectedSourceDescription")]
        [ResourceDisplayName(typeof(DesignResources), "ConfigurationSourceSectionSelectedSourceDisplayName")]
        [Reference(typeof(CustomConfigurationElementCollection<ConfigurationSourceElement, ConfigurationSourceElement>), typeof(ConfigurationSourceElement))]
        [Validation(CommonDesignTime.ValidationTypeNames.SelectedSourceValidator)]
        public string SelectedSource
        {
            get
            {
                return (string)this[selectedSourceProperty];
            }
			set
			{
				this[selectedSourceProperty] = value;
			}
        }

        /// <summary>
        /// Gets or sets the name for the parent configuration source.
        /// </summary>
        [ConfigurationProperty(parentSourceProperty)]
        [ResourceDescription(typeof(DesignResources), "ConfigurationSourceSectionParentSourceDescription")]
        [ResourceDisplayName(typeof(DesignResources), "ConfigurationSourceSectionParentSourceDisplayName")]
        [Reference(typeof(CustomConfigurationElementCollection<ConfigurationSourceElement, ConfigurationSourceElement>), typeof(ConfigurationSourceElement))]
        public string ParentSource
        {
            get
            {
                return (string)this[parentSourceProperty];
            }
            set
            {
                this[parentSourceProperty] = value;
            }
        }

        /// <summary>
        /// Gets the collection of defined configuration sources.
        /// </summary>
        [ConfigurationProperty(sourcesProperty, IsRequired = true)]
        [ConfigurationCollection(typeof(ConfigurationSourceElement))]
        [ResourceDescription(typeof(DesignResources), "ConfigurationSourceSectionSourcesDescription")]
        [ResourceDisplayName(typeof(DesignResources), "ConfigurationSourceSectionSourcesDisplayName")]
        public CustomConfigurationElementCollection<ConfigurationSourceElement, ConfigurationSourceElement> Sources
        {
            get
            {
                return (CustomConfigurationElementCollection<ConfigurationSourceElement, ConfigurationSourceElement>)this[sourcesProperty];
            }           

        }

        /// <summary>
        /// Gets the collection of redirected sections.
        /// </summary>
        [ConfigurationProperty(redirectSectionsProperty)]
        [ResourceDescription(typeof(DesignResources), "ConfigurationSourceSectionRedirectedSectionsDescription")]
        [ResourceDisplayName(typeof(DesignResources), "ConfigurationSourceSectionRedirectedSectionsDisplayName")]
        [ConfigurationCollection(typeof(RedirectedSectionElement))]
        public NamedElementCollection<RedirectedSectionElement> RedirectedSections
        {
            get
            {
                return (NamedElementCollection<RedirectedSectionElement>)this[redirectSectionsProperty];
            }
        }
    }
}
