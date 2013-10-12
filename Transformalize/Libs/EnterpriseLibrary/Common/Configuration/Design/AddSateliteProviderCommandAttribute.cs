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

using System;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design
{
    /// <summary>
    /// Attribute used to overwrite the Add Command for providers that depend on the availability of another block (Sattelite Providers).
    /// </summary>
    public class AddSateliteProviderCommandAttribute : CommandAttribute
    {
        readonly string sectionName;
        readonly Type defaultProviderConfigurationType;
        readonly string defaultProviderConfigurationPropertyName;
        readonly string sateliteProviderReferencePropertyName;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddSateliteProviderCommandAttribute"/> specifying the block dependency by its configuration section name.<br/>
        /// </summary>
        /// <param name="sectionName">The name of the configuran section, used to identify the block dependency.</param>
        public AddSateliteProviderCommandAttribute(string sectionName) 
            : base(CommonDesignTime.CommandTypeNames.AddSatelliteProviderCommand)
        {
            if (string.IsNullOrEmpty(sectionName)) 
                throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, "sectionName");

            this.sectionName = sectionName;
            this.CommandPlacement = CommandPlacement.ContextAdd;
            this.Replace = CommandReplacement.DefaultAddCommandReplacement;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddSateliteProviderCommandAttribute"/> specifying the block dependency by its configuration section name and will assign the value of a default provider to the added element.<br/>
        /// </summary>
        /// <param name="sectionName">The name of the configuran section, used to identify the block dependency.</param>
        /// <param name="defaultProviderConfigurationType">The configuration type of the element that declares the default proviiders name.</param>
        /// <param name="defaultProviderConfigurationPropertyName">The property that will be used to determine the name of the default provider.</param>
        /// <param name="sateliteProviderReferencePropertyName">The property on the created element that will be assigned the name of the default provider.</param>
        public AddSateliteProviderCommandAttribute(string sectionName, Type defaultProviderConfigurationType, string defaultProviderConfigurationPropertyName, string sateliteProviderReferencePropertyName)
            :this(sectionName)
        {
            this.defaultProviderConfigurationType = defaultProviderConfigurationType;
            this.defaultProviderConfigurationPropertyName = defaultProviderConfigurationPropertyName;
            this.sateliteProviderReferencePropertyName = sateliteProviderReferencePropertyName;
        }


        /// <summary>
        /// Gets the section name of the block dependency.
        /// </summary>
        public string SectionName
        {
            get { return sectionName; }
        }

        /// <summary>
        /// If a configuration element exists that specifies a default property, gets the configuration type of the declaring element.
        /// </summary>
        public Type DefaultProviderConfigurationType
        {
            get
            {
                return defaultProviderConfigurationType;
            }
        }

        /// <summary>
        /// If a configuration element exists that specifies a default property, gets the property that contains the name of the default value.
        /// </summary>
        public string DefaultProviderConfigurationPropertyName
        {
            get
            {
                return defaultProviderConfigurationPropertyName;
            }
        }


        /// <summary>
        /// If the provider has a property that should be assigned the name of the default provider, gets the name of the property.
        /// </summary>
        public string SateliteProviderReferencePropertyName
        {
            get
            {
                return sateliteProviderReferencePropertyName;
            }
        }
    }
}
