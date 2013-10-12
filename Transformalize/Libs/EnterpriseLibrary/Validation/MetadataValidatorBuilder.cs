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
using System.Reflection;

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
    /// <summary>
    /// 
    /// </summary>
    public class MetadataValidatorBuilder : ValidatorBuilderBase
    {
        /// <summary>
        /// 
        /// </summary>
        public MetadataValidatorBuilder()
            : this(MemberAccessValidatorBuilderFactory.Default)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberAccessValidatorFactory"></param>
        public MetadataValidatorBuilder(MemberAccessValidatorBuilderFactory memberAccessValidatorFactory)
            : this(memberAccessValidatorFactory, ValidationFactory.DefaultCompositeValidatorFactory)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberAccessValidatorFactory"></param>
        /// <param name="validatorFactory"></param>
        public MetadataValidatorBuilder(
            MemberAccessValidatorBuilderFactory memberAccessValidatorFactory,
            ValidatorFactory validatorFactory)
            : base(memberAccessValidatorFactory, validatorFactory)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ruleset"></param>
        /// <returns></returns>
        public Validator CreateValidator(Type type, string ruleset)
        {
            return CreateValidator(new MetadataValidatedType(type, ruleset));
        }

        #region test only methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ruleset"></param>
        /// <returns></returns>
        public Validator CreateValidatorForType(Type type, string ruleset)
        {
            return CreateValidatorForValidatedElement(
                new MetadataValidatedType(type, ruleset),
                this.GetCompositeValidatorBuilderForType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="ruleset"></param>
        /// <returns></returns>
        public Validator CreateValidatorForProperty(PropertyInfo propertyInfo, string ruleset)
        {
            return CreateValidatorForValidatedElement(new MetadataValidatedElement(propertyInfo, ruleset),
                this.GetCompositeValidatorBuilderForProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <param name="ruleset"></param>
        /// <returns></returns>
        public Validator CreateValidatorForField(FieldInfo fieldInfo, string ruleset)
        {
            return CreateValidatorForValidatedElement(new MetadataValidatedElement(fieldInfo, ruleset),
                this.GetCompositeValidatorBuilderForField);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="ruleset"></param>
        /// <returns></returns>
        public Validator CreateValidatorForMethod(MethodInfo methodInfo, string ruleset)
        {
            return CreateValidatorForValidatedElement(new MetadataValidatedElement(methodInfo, ruleset),
                this.GetCompositeValidatorBuilderForMethod);
        }

        #endregion
    }
}
