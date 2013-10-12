//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Validation Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Configuration
{
    /// <summary>
    /// Configuration section for stored validation information.
    /// </summary>
    /// <seealso cref="ValidatedTypeReference"/>
    [ViewModel(ValidationDesignTime.ViewModelTypeNames.ValidationSectionViewModel)]
    [ResourceDescription(typeof(DesignResources), "ValidationSettingsDescription")]
    [ResourceDisplayName(typeof(DesignResources), "ValidationSettingsDisplayName")]
    public class ValidationSettings : SerializableConfigurationSection
    {
        ///<summary>
        /// Tries to retrieve the <see cref="ValidationSettings"/>.
        ///</summary>
        ///<param name="configurationSource"></param>
        ///<returns></returns>
        public static ValidationSettings TryGet(IConfigurationSource configurationSource)
        {
            if (configurationSource == null) throw new ArgumentNullException("configurationSource");

            return configurationSource.GetSection(ValidationSettings.SectionName) as ValidationSettings;
        }

        /// <summary>
        /// The name used to serialize the configuration section.
        /// </summary>
        public const string SectionName = BlockSectionNames.Validation;

        private const string TypesPropertyName = "";
        /// <summary>
        /// Gets the collection of types for which validation has been configured.
        /// </summary>
        [ConfigurationProperty(TypesPropertyName, IsDefaultCollection = true)]
        [ResourceDescription(typeof(DesignResources), "ValidationSettingsTypesDescription")]
        [ResourceDisplayName(typeof(DesignResources), "ValidationSettingsTypesDisplayName")]
        public ValidatedTypeReferenceCollection Types
        {
            get { return (ValidatedTypeReferenceCollection)this[TypesPropertyName]; }
        }
    }
}
