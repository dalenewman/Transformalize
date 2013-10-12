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

using System.Collections.Generic;
using System.Reflection;

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
    /// <summary>
    /// Represents the description of how validation must be performed on a type.
    /// </summary>
    public interface IValidatedType : IValidatedElement
    {
        /// <summary>
        /// Gets the <see cref="IValidatedElement"/> instances representing the properties on the validated type.
        /// </summary>
        /// <returns>The validated properties.</returns>
        IEnumerable<IValidatedElement> GetValidatedProperties();

        /// <summary>
        /// Gets the <see cref="IValidatedElement"/> instances representing the fields on the validated type.
        /// </summary>
        /// <returns>The validated fields.</returns>
        IEnumerable<IValidatedElement> GetValidatedFields();

        /// <summary>
        /// Gets the <see cref="IValidatedElement"/> instances representing the methods on the validated type.
        /// </summary>
        /// <returns>The validated methods.</returns>
        IEnumerable<IValidatedElement> GetValidatedMethods();

        /// <summary>
        /// Gets the self-validation methods on the validated type.
        /// </summary>
        /// <returns>The self-validation methods.</returns>
        IEnumerable<MethodInfo> GetSelfValidationMethods();
    }
}
