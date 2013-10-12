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

using System.Configuration;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
    /// <summary>
    /// Declaratively instructs the .NET Framework to perform time validation on a configuration property. This class cannot be inherited.
    /// </summary>
    public sealed class NonNegativeOrInfiniteTimeSpanValidatorAttribute : ConfigurationValidatorAttribute
    {
        /// <summary>
        /// Gets the validator attribute instance.
        /// </summary>
        /// <returns>The current <see cref="NonNegativeOrInfiniteTimeSpanValidator" />.</returns>
        public override ConfigurationValidatorBase ValidatorInstance
        {
            get
            {
                return new NonNegativeOrInfiniteTimeSpanValidator();
            }
        }
    }
}
