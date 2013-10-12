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

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators
{
    /// <summary>
    /// Indicates that the kind of composition to use when multiple <see cref="ValidatorAttribute"/> instances
    /// are bound to a language element.
    /// </summary>
    /// <seealso cref="ValidatorAttribute"/>
    [AttributeUsage(AttributeTargets.Property
        | AttributeTargets.Field
        | AttributeTargets.Method
        | AttributeTargets.Parameter
        | AttributeTargets.Class,
        AllowMultiple = true,
        Inherited = false)]
    public sealed class ValidatorCompositionAttribute : BaseValidationAttribute
    {
        private CompositionType compositionType;
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorCompositionAttribute"/> class.
        /// </summary>
        /// <param name="compositionType">The <see cref="CompositionType"/> to be used.</param>
        public ValidatorCompositionAttribute(CompositionType compositionType)
        {
            this.compositionType = compositionType;
        }

        /// <summary>
        /// The <see cref="CompositionType"/> to be used.
        /// </summary>
        public CompositionType CompositionType
        {
            get { return compositionType; }
        }

        private readonly Guid typeId = Guid.NewGuid();

        /// <summary>
        /// Gets a unique identifier for this attribute.
        /// </summary>
        public override object TypeId
        {
            get
            {
                return this.typeId;
            }
        }
    }
}
