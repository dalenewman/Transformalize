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
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design.Validation;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Configuration
{
	/// <summary>
	/// Configuration object to describe an instance of class <see cref="StringLengthValidator"/>.
	/// </summary>
	/// <seealso cref="StringLengthValidator"/>
	/// <seealso cref="ValidatorData"/>
    [ResourceDescription(typeof(DesignResources), "StringLengthValidatorDataDescription")]
    [ResourceDisplayName(typeof(DesignResources), "StringLengthValidatorDataDisplayName")]
	public class StringLengthValidatorData : RangeValidatorData<int>
	{
		/// <summary>
		/// <para>Initializes a new instance of the <see cref="StringLengthValidatorData"/> class.</para>
		/// </summary>
		public StringLengthValidatorData() { Type = typeof(StringLengthValidator); }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="StringLengthValidatorData"/> class with a name.</para>
		/// </summary>
		/// <param name="name">The name for the instance.</param>
		public StringLengthValidatorData(string name)
			: base(name, typeof(StringLengthValidator))
		{ }

		/// <summary>
		/// Creates the <see cref="StringLengthValidator"/> described by the configuration object.
		/// </summary>
		/// <param name="targetType">The type of object that will be validated by the validator.</param>
		/// <returns>The created <see cref="StringLengthValidator"/>.</returns>
		protected override Validator DoCreateValidator(Type targetType)
		{
			return new StringLengthValidator(this.LowerBound, 
				this.LowerBoundType, 
				this.UpperBound, 
				this.UpperBoundType,
				Negated);
		}

        /// <summary>
        /// Overriden in order to apply validation attribute
        /// </summary>
        [Validation(ValidationDesignTime.Validators.RangeBoundValidator)]
        public override int UpperBound
        {
            get
            {
                return base.UpperBound;
            }
            set
            {
                base.UpperBound = value;
            }
        }

        /// <summary>
        /// Overriden in order to apply validation attribute
        /// </summary>
        [Validation(ValidationDesignTime.Validators.RangeBoundValidator)]
        public override int LowerBound
        {
            get
            {
                return base.LowerBound;
            }
            set
            {
                base.LowerBound = value;
            }
        }
	}
}
