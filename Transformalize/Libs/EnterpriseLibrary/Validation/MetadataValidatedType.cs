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
    /// Represents the description of how validation must be performed on a type as defined by attributes.
    /// </summary>
    /// <remarks>
    /// This class is a flyweight, so instances should not be kept for later use.
    /// </remarks>
    public class MetadataValidatedType : MetadataValidatedElement, IValidatedType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataValidatedType"/> class for a type and a ruleset.
        /// </summary>
        /// <param name="targetType">The target type.</param>
        /// <param name="ruleset">The ruleset.</param>
        public MetadataValidatedType(Type targetType, string ruleset)
            : base(targetType, ruleset)
        { }

        IEnumerable<IValidatedElement> IValidatedType.GetValidatedProperties()
        {
            MetadataValidatedElement flyweight = new MetadataValidatedElement(this.Ruleset);

            foreach (PropertyInfo propertyInfo in this.TargetType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (ValidationReflectionHelper.IsValidProperty(propertyInfo))
                {
                    flyweight.UpdateFlyweight(propertyInfo);
                    yield return flyweight;
                }
            }
        }

        IEnumerable<IValidatedElement> IValidatedType.GetValidatedFields()
        {
            MetadataValidatedElement flyweight = new MetadataValidatedElement(this.Ruleset);

            foreach (FieldInfo fieldInfo in this.TargetType.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                flyweight.UpdateFlyweight(fieldInfo);
                yield return flyweight;
            }
        }

        IEnumerable<IValidatedElement> IValidatedType.GetValidatedMethods()
        {
            MetadataValidatedElement flyweight = new MetadataValidatedElement(this.Ruleset);

            foreach (MethodInfo methodInfo in this.TargetType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                ParameterInfo[] parameters = methodInfo.GetParameters();

                if (ValidationReflectionHelper.IsValidMethod(methodInfo))
                {
                    flyweight.UpdateFlyweight(methodInfo);
                    yield return flyweight;
                }
            }
        }

        IEnumerable<MethodInfo> IValidatedType.GetSelfValidationMethods()
        {
            Type type = this.TargetType;

            if (ValidationReflectionHelper.GetCustomAttributes(type, typeof(HasSelfValidationAttribute), false).Length == 0)
                yield break;		// no self validation for the current type, ignore type

            foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                bool hasReturnType = methodInfo.ReturnType != typeof(void);
                ParameterInfo[] parameters = methodInfo.GetParameters();

                if (!hasReturnType && parameters.Length == 1 && parameters[0].ParameterType == typeof(ValidationResults))
                {
                    foreach (SelfValidationAttribute attribute
                        in ValidationReflectionHelper.GetCustomAttributes(methodInfo, typeof(SelfValidationAttribute), false))
                    {
                        if (this.Ruleset.Equals(attribute.Ruleset))
                        {
                            yield return methodInfo;
                            continue;	// done with current method
                        }
                    }
                }
            }
        }
    }
}
