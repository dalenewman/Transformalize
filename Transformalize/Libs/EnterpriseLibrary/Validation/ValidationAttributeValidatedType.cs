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
using System.Linq;
using System.Reflection;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design.Validation;

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
    /// <summary>
    /// Represents the description of how validation must be performed on a type as defined by 
    /// <see cref="ValidationAttribute"/> attached to the fields and properties in the type.
    /// </summary>
    /// <seealso cref="ValidationAttribute"/>
    /// <seealso cref="ValidationAttributeValidatedElement"/>
    /// <seealso cref="IValidatedType"/>
    public class ValidationAttributeValidatedType : ValidationAttributeValidatedElement, IValidatedType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationAttributeValidatedType"/> for a <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type to represent.</param>
        public ValidationAttributeValidatedType(Type type)
            : base(type, type)
        { }

        IEnumerable<IValidatedElement> IValidatedType.GetValidatedProperties()
        {
            foreach (PropertyInfo propertyInfo in
                ((IValidatedElement)this).TargetType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (ValidationReflectionHelper.IsValidProperty(propertyInfo))
                {
                    yield return new ValidationAttributeValidatedElement(propertyInfo);
                }
            }
        }

        IEnumerable<IValidatedElement> IValidatedType.GetValidatedFields()
        {
            foreach (FieldInfo fieldInfo in
                ((IValidatedElement)this).TargetType.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (ValidationReflectionHelper.IsValidField(fieldInfo))
                {
                    yield return new ValidationAttributeValidatedElement(fieldInfo);
                }
            }
        }

        IEnumerable<IValidatedElement> IValidatedType.GetValidatedMethods()
        {
            return Enumerable.Empty<IValidatedElement>(); ;
        }

        IEnumerable<MethodInfo> IValidatedType.GetSelfValidationMethods()
        {
            return Enumerable.Empty<MethodInfo>(); ;
        }
    }
}
