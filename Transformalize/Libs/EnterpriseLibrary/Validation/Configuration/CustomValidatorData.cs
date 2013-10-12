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
using System.Collections.Specialized;
using System.Configuration;
using System.ComponentModel;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design.Validation;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Configuration
{
	/// <summary>
	/// Configuration object to describe an instance of custom <see cref="Validator"/> class.
	/// </summary>
	/// <remarks>
	/// Custom <see cref="Validator"/> classes must implement a constructor with with name and value collection parameters.
	/// </remarks>
    [ResourceDescription(typeof(DesignResources), "CustomValidatorDataDescription")]
    [ResourceDisplayName(typeof(DesignResources), "CustomValidatorDataDisplayName")]
    [TypePickingCommand(TitleResourceName = "CustomValidatorDataDisplayName", TitleResourceType = typeof(DesignResources), Replace=CommandReplacement.DefaultAddCommandReplacement)]
	public class CustomValidatorData : ValidatorData,
		IHelperAssistedCustomConfigurationData<CustomValidatorData>
	{
		private readonly CustomProviderDataHelper<CustomValidatorData> helper;

		/// <summary>
		/// Initializes with default values.
		/// </summary>
		public CustomValidatorData()
		{
			helper = new CustomProviderDataHelper<CustomValidatorData>(this);
		}

		/// <summary>
		/// Initializes with name and provider type.
		/// </summary>
		public CustomValidatorData(string name, Type type)
		{
			helper = new CustomProviderDataHelper<CustomValidatorData>(this);
			Name = name;
			Type = type;
		}

		/// <summary>
		/// Initializes with name and provider type.
		/// </summary>
		public CustomValidatorData(string name, string typeName)
		{
			helper = new CustomProviderDataHelper<CustomValidatorData>(this);
			Name = name;
			TypeName = typeName;
		}

		/// <summary>
		/// Sets the attribute value for a key.
		/// </summary>
		/// <param name="key">The attribute name.</param>
		/// <param name="value">The attribute value.</param>
		public void SetAttributeValue(string key, string value)
		{
			helper.HandleSetAttributeValue(key, value);
		}


        /// <summary>
        /// Overridden in order to apply <see cref="BrowsableAttribute"/>.
        /// </summary>
        [Browsable(true)]
        [Editor(CommonDesignTime.EditorTypes.TypeSelector, CommonDesignTime.EditorTypes.UITypeEditor)]
        [BaseType(typeof(Validator), typeof(CustomValidatorData))]
        [ResourceDescription(typeof(DesignResources), "CustomValidatorDataTypeNameDescription")]
        [ResourceDisplayName(typeof(DesignResources), "CustomValidatorDataTypeNameDisplayName")]
        public override string TypeName
        {
            get { return base.TypeName; }
            set { base.TypeName = value; }
        }


		/// <summary>
		/// Gets or sets custom configuration attributes.
		/// </summary>
        [Validation(ValidationDesignTime.Validators.NameValueCollectionValidator)]
		public NameValueCollection Attributes
		{
			get { return helper.Attributes; }
		}

		/// <summary>
		/// Gets a <see cref="ConfigurationPropertyCollection"/> of the properties that are defined for 
		/// this configuration element when implemented in a derived class. 
		/// </summary>
		/// <value>
		/// A <see cref="ConfigurationPropertyCollection"/> of the properties that are defined for this
		/// configuration element when implemented in a derived class. 
		/// </value>
		protected override ConfigurationPropertyCollection Properties
		{
			get { return helper.Properties; }
		}

		/// <summary>
		/// Modifies the <see cref="CustomValidatorData"/> object to remove all values that should not be saved. 
		/// </summary>
		/// <param name="sourceElement">A <see cref="ConfigurationElement"/> object at the current level containing a merged view of the properties.</param>
		/// <param name="parentElement">A parent <see cref="ConfigurationElement"/> object or <see langword="null"/> if this is the top level.</param>		
		/// <param name="saveMode">One of the <see cref="ConfigurationSaveMode"/> values.</param>
		protected override void Unmerge(ConfigurationElement sourceElement, ConfigurationElement parentElement, ConfigurationSaveMode saveMode)
		{
			helper.HandleUnmerge(sourceElement, parentElement, saveMode);
		}

		/// <summary>
		/// Resets the internal state of the <see cref="CustomValidatorData"/> object, 
		/// including the locks and the properties collection.
		/// </summary>
		/// <param name="parentElement">The parent element.</param>
		protected override void Reset(ConfigurationElement parentElement)
		{
			helper.HandleReset(parentElement);
		}

		/// <summary>
		/// Indicates whether this configuration element has been modified since it was last 
		/// saved or loaded when implemented in a derived class.
		/// </summary>
		/// <returns><see langword="true"/> if the element has been modified; otherwise, <see langword="false"/>. </returns>
		protected override bool IsModified()
		{
			return helper.HandleIsModified();
		}

		/// <summary>
		/// Called when an unknown attribute is encountered while deserializing the <see cref="CustomValidatorData"/> object.
		/// </summary>
		/// <param name="name">The name of the unrecognized attribute.</param>
		/// <param name="value">The value of the unrecognized attribute.</param>
		/// <returns><see langword="true"/> if the processing of the element should continue; otherwise, <see langword="false"/>.</returns>
		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			return helper.HandleOnDeserializeUnrecognizedAttribute(name, value);
		}

		/// <summary>
		/// Gets the helper.
		/// </summary>
		CustomProviderDataHelper<CustomValidatorData> IHelperAssistedCustomConfigurationData<CustomValidatorData>.Helper
		{
			get { return helper; }
		}

		/// <summary>Invokes the inherited behavior.</summary>
		object IHelperAssistedCustomConfigurationData<CustomValidatorData>.BaseGetPropertyValue(ConfigurationProperty property)
		{
			return base[property];
		}

		/// <summary>Invokes the inherited behavior.</summary>
		void IHelperAssistedCustomConfigurationData<CustomValidatorData>.BaseSetPropertyValue(ConfigurationProperty property, object value)
		{
			base[property] = value;
		}

		/// <summary>Invokes the inherited behavior.</summary>
		void IHelperAssistedCustomConfigurationData<CustomValidatorData>.BaseUnmerge(ConfigurationElement sourceElement,
					ConfigurationElement parentElement,
					ConfigurationSaveMode saveMode)
		{
			base.Unmerge(sourceElement, parentElement, saveMode);
		}

		/// <summary>Invokes the inherited behavior.</summary>
		void IHelperAssistedCustomConfigurationData<CustomValidatorData>.BaseReset(ConfigurationElement parentElement)
		{
			base.Reset(parentElement);
		}

		/// <summary>Invokes the inherited behavior.</summary>
		bool IHelperAssistedCustomConfigurationData<CustomValidatorData>.BaseIsModified()
		{
			return base.IsModified();
		}

		/// <summary>
		/// Creates the <see cref="Validator"/> described by the configuration object.
		/// </summary>
		/// <param name="targetType">The type of object that will be validated by the validator.</param>
		/// <returns>The created <see cref="Validator"/>.</returns>
		/// <seealso cref="Validator"/>
		protected override Validator DoCreateValidator(Type targetType)
		{
			Validator validator
				= (Validator)Activator.CreateInstance(this.Type, this.Attributes);

			return validator;
		}
	}
}
