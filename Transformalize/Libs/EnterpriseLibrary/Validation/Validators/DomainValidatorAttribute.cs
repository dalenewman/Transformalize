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
	/// Represents a <see cref="DomainValidatorAttribute"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property
		| AttributeTargets.Field
		| AttributeTargets.Method
		| AttributeTargets.Parameter,
		AllowMultiple = true,
		Inherited = false)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019",
		Justification = "Fields are used internally")]
	public sealed class DomainValidatorAttribute : ValueValidatorAttribute
	{
		private object[] domain;

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="DomainValidatorAttribute"/>.</para>
		/// </summary>
		public DomainValidatorAttribute()
			: this(new object[0])
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="DomainValidatorAttribute"/>.</para>
		/// </summary>
		/// <param name="domain">List of values to be used in the validation.</param>
		public DomainValidatorAttribute(params object[] domain)
			: base()
		{
			ValidatorArgumentsValidatorHelper.ValidateDomainValidator(domain);

			this.domain = domain;
		}

        /// <summary>
        /// List of values to be used in the validation.
        /// </summary>
	    public object[] Domain
	    {
	        get { return domain; }
	    }

		/// <summary>
		/// Creates the <see cref="DomainValidatorAttribute"/> described by the attribute object.
		/// </summary>
		/// <param name="targetType">The type of object that will be validated by the validator.</param>
		/// <remarks>This operation must be overriden by subclasses.</remarks>
		/// <returns>The created <see cref="DomainValidatorAttribute"/>.</returns>
		protected override Validator DoCreateValidator(Type targetType)
		{
			return new DomainValidator<object>(Negated, Domain);
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
