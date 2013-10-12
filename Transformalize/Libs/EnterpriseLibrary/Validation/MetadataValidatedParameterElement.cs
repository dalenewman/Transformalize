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
using System.Collections.Generic;
using System.Reflection;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
    /// <summary>
    /// Describes how validation must be performed on a parameter as defined by attributes.
    /// </summary>
    public class MetadataValidatedParameterElement : IValidatedElement
    {
        private ParameterInfo parameterInfo;
        private IgnoreNullsAttribute ignoreNullsAttribute;
        private ValidatorCompositionAttribute validatorCompositionAttribute;

        /// <summary>
        /// Gets the validator descriptors for the validated element.
        /// </summary>
        public IEnumerable<IValidatorDescriptor> GetValidatorDescriptors()
        {
            if (parameterInfo != null)
            {
                foreach (object attribute in parameterInfo.GetCustomAttributes(typeof(ValidatorAttribute), false))
                {
                    yield return (IValidatorDescriptor)attribute;
                }
            }
        }

        /// <summary>
        /// Gets the value indicating how the validators for the validated element.
        /// </summary>
        public CompositionType CompositionType
        {
            get
            {
                return this.validatorCompositionAttribute != null
                    ? this.validatorCompositionAttribute.CompositionType
                    : CompositionType.And;
            }
        }

        /// <summary>
        /// Gets the message to use for the validator composing the validators for element.
        /// </summary>
        public string CompositionMessageTemplate
        {
            get
            {
                return this.validatorCompositionAttribute != null
                    ? this.validatorCompositionAttribute.GetMessageTemplate()
                    : null;
            }
        }

        /// <summary>
        /// Gets the tag to use for the validator composing the validators for the element.
        /// </summary>
        public string CompositionTag
        {
            get
            {
                return this.validatorCompositionAttribute != null
                    ? this.validatorCompositionAttribute.Tag
                    : null;
            }
        }

        /// <summary>
        /// Gets the flag indicating whether null values should be ignored.
        /// </summary>
        public bool IgnoreNulls
        {
            get
            {
                return this.ignoreNullsAttribute != null;
            }
        }

        /// <summary>
        /// Gets the message for null failures.
        /// </summary>
        public string IgnoreNullsMessageTemplate
        {
            get
            {
                return this.ignoreNullsAttribute != null
                    ? this.ignoreNullsAttribute.GetMessageTemplate()
                    : null;
            }
        }

        /// <summary>
        /// Gets the tag for null failures.
        /// </summary>
        public string IgnoreNullsTag
        {
            get
            {
                return this.ignoreNullsAttribute != null
                    ? this.ignoreNullsAttribute.Tag
                    : null;
            }
        }

        /// <summary>
        /// Gets the validated member.
        /// </summary>
        public MemberInfo MemberInfo
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the type of the validated member.
        /// </summary>
        public Type TargetType
        {
            get { return null; }
        }

        /// <summary>
        /// Updates the flyweight for a parameter.
        /// </summary>
        /// <param name="parameterInfo">The parameter.</param>
        public void UpdateFlyweight(ParameterInfo parameterInfo)
        {
            this.parameterInfo = parameterInfo;
            this.ignoreNullsAttribute
                = ValidationReflectionHelper.ExtractValidationAttribute<IgnoreNullsAttribute>(parameterInfo, string.Empty);
            this.validatorCompositionAttribute
                = ValidationReflectionHelper.ExtractValidationAttribute<ValidatorCompositionAttribute>(parameterInfo, string.Empty);
        }
    }
}
