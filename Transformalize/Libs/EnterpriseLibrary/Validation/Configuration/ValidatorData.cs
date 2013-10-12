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
using System.ComponentModel;
using System.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Common;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Configuration
{
    /// <summary>
    /// Base class for configuration objects describing validators.
    /// </summary>
    [ViewModel(ValidationDesignTime.ViewModelTypeNames.ValidatorDataViewModel)]
    public class ValidatorData : NameTypeConfigurationElement, IValidatorDescriptor
    {
        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ValidatorData"/> class.</para>
        /// </summary>
        public ValidatorData()
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ValidatorData"/> class with a name and a type.</para>
        /// </summary>
        /// <param name="name">The name for the instance.</param>
        /// <param name="validatorType">The type of the represented validator.</param>
        protected internal ValidatorData(string name, Type validatorType)
            : base(name, validatorType)
        { }

        private const string MessageTemplatePropertyName = "messageTemplate";
        /// <summary>
        /// Gets or sets the message template to use when logging validation results.
        /// </summary>
        /// <remarks>
        /// Either the <see cref="ValidatorData.MessageTemplate"/> or the 
        /// pair <see cref="ValidatorData.MessageTemplateResourceName"/> 
        /// and <see cref="ValidatorData.MessageTemplateResourceTypeName"/> can be used to 
        /// provide a message template for the represented validator.
        /// <para/>
        /// If both the template and the resource reference are specified, the template will be used.
        /// </remarks>
        /// <seealso cref="ValidatorData.MessageTemplateResourceName"/> 
        /// <seealso cref="ValidatorData.MessageTemplateResourceTypeName"/>
        [ConfigurationProperty(MessageTemplatePropertyName)]
        [Editor(CommonDesignTime.EditorTypes.MultilineText, CommonDesignTime.EditorTypes.FrameworkElement)]
        [ResourceDescription(typeof(DesignResources), "ValidatorDataMessageTemplateDescription")]
        [ResourceDisplayName(typeof(DesignResources), "ValidatorDataMessageTemplateDisplayName")]
        public virtual string MessageTemplate
        {
            get { return (string)this[MessageTemplatePropertyName]; }
            set { this[MessageTemplatePropertyName] = value; }
        }

        private const string MessageTemplateResourceNamePropertyName = "messageTemplateResourceName";
        /// <summary>
        /// Gets or sets the name of the resource to retrieve the message template to use when logging validation results.
        /// </summary>
        /// <remarks>
        /// Used in combination with <see cref="ValidatorData.MessageTemplateResourceTypeName"/>.
        /// <para/>
        /// Either the <see cref="ValidatorData.MessageTemplate"/> or the 
        /// pair <see cref="ValidatorData.MessageTemplateResourceName"/> 
        /// and <see cref="ValidatorData.MessageTemplateResourceTypeName"/> can be used to 
        /// provide a message template for the represented validator.
        /// <para/>
        /// If both the template and the resource reference are specified, the template will be used.
        /// </remarks>
        /// <seealso cref="ValidatorData.MessageTemplate"/> 
        /// <seealso cref="ValidatorData.MessageTemplateResourceTypeName"/>
        [ConfigurationProperty(MessageTemplateResourceNamePropertyName)]
        [ResourceDescription(typeof(DesignResources), "ValidatorDataMessageTemplateResourceNameDescription")]
        [ResourceDisplayName(typeof(DesignResources), "ValidatorDataMessageTemplateResourceNameDisplayName")]
        [Category("CategoryLocalization")]
        public virtual string MessageTemplateResourceName
        {
            get { return (string)this[MessageTemplateResourceNamePropertyName]; }
            set { this[MessageTemplateResourceNamePropertyName] = value; }
        }

        private const string MessageTemplateResourceTypeNamePropertyName = "messageTemplateResourceType";
        /// <summary>
        /// Gets or sets the name of the type to retrieve the message template to use when logging validation results.
        /// </summary>
        /// <remarks>
        /// Used in combination with <see cref="ValidatorData.MessageTemplateResourceName"/>.
        /// <para/>
        /// Either the <see cref="ValidatorData.MessageTemplate"/> or the 
        /// pair <see cref="ValidatorData.MessageTemplate"/> 
        /// and <see cref="ValidatorData.MessageTemplateResourceTypeName"/> can be used to 
        /// provide a message template for the represented validator.
        /// <para/>
        /// If both the template and the resource reference are specified, the template will be used.
        /// </remarks>
        /// <seealso cref="ValidatorData.MessageTemplate"/> 
        /// <seealso cref="ValidatorData.MessageTemplateResourceName"/>
        [ConfigurationProperty(MessageTemplateResourceTypeNamePropertyName)]
        [ResourceDescription(typeof(DesignResources), "ValidatorDataMessageTemplateResourceTypeNameDescription")]
        [ResourceDisplayName(typeof(DesignResources), "ValidatorDataMessageTemplateResourceTypeNameDisplayName")]
        [Category("CategoryLocalization")]
        [Editor(CommonDesignTime.EditorTypes.TypeSelector, CommonDesignTime.EditorTypes.UITypeEditor)]
        [BaseType(typeof(object))]
        public virtual string MessageTemplateResourceTypeName
        {
            get { return (string)this[MessageTemplateResourceTypeNamePropertyName]; }
            set { this[MessageTemplateResourceTypeNamePropertyName] = value; }
        }

        private const string TagPropertyName = "tag";
        /// <summary>
        /// Gets or sets the tag that will characterize the results logged by the represented validator.
        /// </summary>
        [ConfigurationProperty(TagPropertyName)]
        [ResourceDescription(typeof(DesignResources), "ValidatorDataTagDescription")]
        [ResourceDisplayName(typeof(DesignResources), "ValidatorDataTagDisplayName")]
        public virtual string Tag
        {
            get { return (string)this[TagPropertyName]; }
            set { this[TagPropertyName] = value; }
        }

        /// <summary>
        /// Creates the <see cref="Validator"/> described by the configuration object.
        /// </summary>
        /// <param name="targetType">The type of object that will be validated by the validator.</param>
        /// <param name="ownerType">The type of the object from which the value to validate is extracted.</param>
        /// <param name="memberValueAccessBuilder">The <see cref="MemberValueAccessBuilder"/> to use for validators that
        /// require access to properties.</param>
        /// <param name="validatorFactory">Factory to use when building nested validators.</param>
        /// <returns>The created <see cref="Validator"/>.</returns>
        Validator IValidatorDescriptor.CreateValidator(
            Type targetType,
            Type ownerType,
            MemberValueAccessBuilder memberValueAccessBuilder,
            ValidatorFactory validatorFactory)
        {
            Validator validator = DoCreateValidator(targetType, ownerType, memberValueAccessBuilder, validatorFactory);
            validator.Tag = string.IsNullOrEmpty(this.Tag) ? null : this.Tag;
            validator.MessageTemplate = GetMessageTemplate();

            return validator;
        }

        /// <summary>
        /// Creates the <see cref="Validator"/> described by the configuration object.
        /// </summary>
        /// <param name="targetType">The type of object that will be validated by the validator.</param>
        /// <param name="ownerType">The type of the object from which the value to validate is extracted.</param>
        /// <param name="memberValueAccessBuilder">The <see cref="MemberValueAccessBuilder"/> to use for validators that
        /// require access to properties.</param>
        /// <param name="validatorFactory">Factory to use when building nested validators.</param>
        /// <returns>The created <see cref="Validator"/>.</returns>
        /// <remarks>
        /// The default implementation invokes <see cref="ValidatorData.DoCreateValidator(Type)"/>. Subclasses requiring access to all
        /// the parameters or this method may override it instead of <see cref="ValidatorData.DoCreateValidator(Type)"/>.
        /// </remarks>
        protected virtual Validator DoCreateValidator(
            Type targetType,
            Type ownerType, MemberValueAccessBuilder
            memberValueAccessBuilder,
            ValidatorFactory validatorFactory)
        {
            return DoCreateValidator(targetType);
        }

        /// <summary>
        /// Creates the <see cref="Validator"/> described by the configuration object.
        /// </summary>
        /// <param name="targetType">The type of object that will be validated by the validator.</param>
        /// <remarks>This operation must be overriden by subclasses.</remarks>
        /// <returns>The created <see cref="Validator"/>.</returns>
        protected virtual Validator DoCreateValidator(Type targetType)
        {
            throw new NotImplementedException(Resources.MustImplementOperation);
        }

        /// <summary>
        /// Returns the message template for the represented validator.
        /// </summary>
        /// <remarks>
        /// The textual message is given precedence over the resource based mechanism.
        /// </remarks>
        public string GetMessageTemplate()
        {
            if (!string.IsNullOrEmpty(this.MessageTemplate))
            {
                return this.MessageTemplate;
            }
            Type messageTemplateResourceType = this.GetMessageTemplateResourceType();
            if (null != messageTemplateResourceType)
            {
                return ResourceStringLoader.LoadString(messageTemplateResourceType.FullName,
                    this.MessageTemplateResourceName,
                    messageTemplateResourceType.Assembly);
            }

            return null;
        }

        private Type GetMessageTemplateResourceType()
        {
            if (!string.IsNullOrEmpty(this.MessageTemplateResourceTypeName))
            {
                return Type.GetType(this.MessageTemplateResourceTypeName);
            }

            return null;
        }
    }
}
