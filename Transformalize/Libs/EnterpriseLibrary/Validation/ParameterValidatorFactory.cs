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

using System.Reflection;

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
    /// <summary>
    /// A special factory class that creates Validator objects based on
    /// attributes specified on a method parameter.
    /// </summary>
    public static class ParameterValidatorFactory
    {
        /// <summary>
        /// Create a <see cref="Validator"/> instance based on the validation
        /// attributes on the given parameter.
        /// </summary>
        /// <remarks>This factory method ignores configuration and rulesets. For parameters,
        /// only attribute based configuration is supported at this time.</remarks>
        /// <param name="paramInfo">The <see cref="ParameterInfo"/> for the parameter to construct a validator for.</param>
        /// <returns>The <see cref="Validator"/></returns>
        public static Validator CreateValidator(ParameterInfo paramInfo)
        {
            MetadataValidatedParameterElement parameterElement = new MetadataValidatedParameterElement();
            parameterElement.UpdateFlyweight(paramInfo);
            CompositeValidatorBuilder compositeBuilder = new CompositeValidatorBuilder(parameterElement);
            foreach (IValidatorDescriptor descriptor in parameterElement.GetValidatorDescriptors())
            {
                compositeBuilder.AddValueValidator(
                    descriptor.CreateValidator(
                        paramInfo.ParameterType,
                        null,
                        null,
                        ValidationFactory.DefaultCompositeValidatorFactory));
            }
            return compositeBuilder.GetValidator();
        }
    }
}
