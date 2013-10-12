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
    /// Represents the description of how validation must be performed on a language element.
    /// </summary>
    public interface IValidatedElement
    {
        /// <summary>
        /// Gets the validator descriptors for the validated element.
        /// </summary>
        IEnumerable<IValidatorDescriptor> GetValidatorDescriptors();

        /// <summary>
        /// Gets the value indicating how the validators for the validated element.
        /// </summary>
        CompositionType CompositionType { get; }

        /// <summary>
        /// Gets the message to use for the validator composing the validators for element.
        /// </summary>
        string CompositionMessageTemplate { get; }

        /// <summary>
        /// Gets the tag to use for the validator composing the validators for the element.
        /// </summary>
        string CompositionTag { get; }

        /// <summary>
        /// Gets the flag indicating whether null values should be ignored.
        /// </summary>
        bool IgnoreNulls { get; }

        /// <summary>
        /// Gets the message for null failures.
        /// </summary>
        string IgnoreNullsMessageTemplate { get; }

        /// <summary>
        /// Gets the tag for null failures.
        /// </summary>
        string IgnoreNullsTag { get; }

        /// <summary>
        /// Gets the validated member.
        /// </summary>
        MemberInfo MemberInfo { get; }

        /// <summary>
        /// Gets the type of the validated member.
        /// </summary>
        Type TargetType { get; }
    }
}
