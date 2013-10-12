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
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Transformalize.Libs.EnterpriseLibrary.Common;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators
{
    /// <summary>
    /// Base class for validation related attributes.
    /// </summary>
    /// <remarks>
    /// Holds shared information about validation.
    /// </remarks>
    public abstract class BaseValidationAttribute : ValidationAttribute
    {
        private string ruleset;
        private string messageTemplate;
        private string messageTemplateResourceName;
        private Type messageTemplateResourceType;
        private string tag;

        /// <summary>
        /// Returns the message template for the represented validator.
        /// </summary>
        /// <remarks>
        /// The textual message is given precedence over the resource based mechanism.
        /// </remarks>
        public string GetMessageTemplate()
        {
            if (null != this.messageTemplate)
            {
                return this.messageTemplate;
            }
            if (null != this.messageTemplateResourceName && null != this.messageTemplateResourceType)
            {
                return ResourceStringLoader.LoadString(this.messageTemplateResourceType.FullName,
                    this.messageTemplateResourceName,
                    this.messageTemplateResourceType.Assembly);
            }
            if (null != this.messageTemplateResourceName || null != this.messageTemplateResourceType)
            {
                throw new InvalidOperationException(Resources.ExceptionPartiallyDefinedResourceForMessageTemplate);
            }
            return null;
        }

        /// <summary>
        /// Gets or set the ruleset to which the represented validator belongs.
        /// </summary>
        public string Ruleset
        {
            get { return this.ruleset != null ? this.ruleset : string.Empty; }
            set { this.ruleset = value; }
        }

        /// <summary>
        /// Gets or sets the message template to use when logging validation results.
        /// </summary>
        /// <remarks>
        /// Either the <see cref="BaseValidationAttribute.MessageTemplate"/> or the 
        /// pair <see cref="BaseValidationAttribute.MessageTemplateResourceName"/> 
        /// and <see cref="BaseValidationAttribute.MessageTemplateResourceType"/> can be used to 
        /// provide a message template for the represented validator.
        /// <para/>
        /// If both mechanisms are specified an exception occurs.
        /// </remarks>
        /// <seealso cref="BaseValidationAttribute.MessageTemplateResourceName"/> 
        /// <seealso cref="BaseValidationAttribute.MessageTemplateResourceType"/>
        /// <exception cref="InvalidOperationException">when setting the value and either 
        /// <see cref="BaseValidationAttribute.MessageTemplateResourceName"/> or
        /// <see cref="BaseValidationAttribute.MessageTemplateResourceType"/> have been set already.</exception>
        public string MessageTemplate
        {
            get { return messageTemplate; }
            set
            {
                if (this.messageTemplateResourceName != null)
                {
                    throw new InvalidOperationException(Resources.ExceptionCannotSetResourceMessageTemplatesIfResourceTemplateIsSet);
                }
                if (this.messageTemplateResourceType != null)
                {
                    throw new InvalidOperationException(Resources.ExceptionCannotSetResourceMessageTemplatesIfResourceTemplateIsSet);
                }
                messageTemplate = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the resource to retrieve the message template to use when logging validation results.
        /// </summary>
        /// <remarks>
        /// Used in combination with <see cref="BaseValidationAttribute.MessageTemplateResourceType"/>.
        /// <para/>
        /// Either the <see cref="BaseValidationAttribute.MessageTemplate"/> or the 
        /// pair <see cref="BaseValidationAttribute.MessageTemplateResourceName"/> 
        /// and <see cref="BaseValidationAttribute.MessageTemplateResourceType"/> can be used to 
        /// provide a message template for the represented validator.
        /// <para/>
        /// If both mechanisms are specified an exception occurs.
        /// </remarks>
        /// <seealso cref="BaseValidationAttribute.MessageTemplate"/> 
        /// <seealso cref="BaseValidationAttribute.MessageTemplateResourceType"/>
        /// <exception cref="InvalidOperationException">when setting the value and the 
        /// <see cref="BaseValidationAttribute.MessageTemplate"/> has been set already.</exception>
        public string MessageTemplateResourceName
        {
            get { return messageTemplateResourceName; }
            set
            {
                if (this.messageTemplate != null)
                {
                    throw new InvalidOperationException(Resources.ExceptionCannotSetResourceBasedMessageTemplatesIfTemplateIsSet);
                }
                messageTemplateResourceName = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the type to retrieve the message template to use when logging validation results.
        /// </summary>
        /// <remarks>
        /// Used in combination with <see cref="BaseValidationAttribute.MessageTemplateResourceName"/>.
        /// <para/>
        /// Either the <see cref="BaseValidationAttribute.MessageTemplate"/> or the 
        /// pair <see cref="BaseValidationAttribute.MessageTemplate"/> 
        /// and <see cref="BaseValidationAttribute.MessageTemplateResourceType"/> can be used to 
        /// provide a message template for the represented validator.
        /// <para/>
        /// If both mechanisms are specified an exception occurs.
        /// </remarks>
        /// <seealso cref="BaseValidationAttribute.MessageTemplate"/> 
        /// <seealso cref="BaseValidationAttribute.MessageTemplateResourceName"/>
        /// <exception cref="InvalidOperationException">when setting the value and the 
        /// <see cref="BaseValidationAttribute.MessageTemplate"/> has been set already.</exception>
        public Type MessageTemplateResourceType
        {
            get { return messageTemplateResourceType; }
            set
            {
                if (this.messageTemplate != null)
                {
                    throw new InvalidOperationException(Resources.ExceptionCannotSetResourceBasedMessageTemplatesIfTemplateIsSet);
                }
                messageTemplateResourceType = value;
            }
        }

        /// <summary>
        /// Gets or sets the tag that will characterize the results logged by the represented validator.
        /// </summary>
        public string Tag
        {
            get { return this.tag; }
            set { this.tag = value; }
        }

        /// <summary>
        /// Determines whether the specified value of the object is valid.
        /// </summary>
        /// <param name="value">The value of the specified validation object on which the 
        /// <see cref="System.ComponentModel.DataAnnotations.ValidationAttribute "/> is declared.</param>
        /// <returns><see langword="true"/> if the specified value is valid; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="NotSupportedException">when invoked on an attribute with a non-null ruleset.</exception>
        public override bool IsValid(object value)
        {
            if (!string.IsNullOrEmpty(this.Ruleset))
            {
                return true;
            }

            throw new NotSupportedException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.ExceptionValidationAttributeNotSupported,
                    this.GetType().Name));
        }
    }
}
