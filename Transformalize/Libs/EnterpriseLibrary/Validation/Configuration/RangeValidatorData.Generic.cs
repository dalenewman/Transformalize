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
using System.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Configuration
{
	/// <summary>
	/// Describes a <see cref="RangeValidator{T}"/>.
	/// </summary>
	public abstract class RangeValidatorData<T> : ValueValidatorData
		where T : IComparable<T>
	{
		/// <summary>
		/// <para>Initializes a new instance of the <see cref="RangeValidatorData{T}"/> class.</para>
		/// </summary>
		protected RangeValidatorData()
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="RangeValidatorData{T}"/> class with a name.</para>
		/// </summary>
		protected RangeValidatorData(string name, Type type)
			: base(name, type)
		{ }

		private const string LowerBoundPropertyName = "lowerBound";
		/// <summary>
		/// Gets or sets the lower bound for the represented validator.
		/// </summary>
		[ConfigurationProperty(LowerBoundPropertyName)]
        [ResourceDescription(typeof(DesignResources), "RangeValidatorDataLowerBoundDescription")]
        [ResourceDisplayName(typeof(DesignResources), "RangeValidatorDataLowerBoundDisplayName")]
        public virtual T LowerBound
		{
			get { return (T)this[LowerBoundPropertyName]; }
			set { this[LowerBoundPropertyName] = value; }
		}

		private const string LowerBoundTypePropertyName = "lowerBoundType";
		/// <summary>
		/// Gets or sets the lower bound type for the represented validator.
		/// </summary>
		[ConfigurationProperty(LowerBoundTypePropertyName, DefaultValue = RangeBoundaryType.Ignore)]
        [ResourceDescription(typeof(DesignResources), "RangeValidatorDataLowerBoundTypeDescription")]
        [ResourceDisplayName(typeof(DesignResources), "RangeValidatorDataLowerBoundTypeDisplayName")]
		public RangeBoundaryType LowerBoundType
		{
			get { return (RangeBoundaryType)this[LowerBoundTypePropertyName]; }
			set { this[LowerBoundTypePropertyName] = value; }
		}

		private const string UpperBoundPropertyName = "upperBound";
		/// <summary>
		/// Gets or sets the upper bound for the represented validator.
		/// </summary>
		[ConfigurationProperty(UpperBoundPropertyName)]
        [ResourceDescription(typeof(DesignResources), "RangeValidatorDataUpperBoundDescription")]
        [ResourceDisplayName(typeof(DesignResources), "RangeValidatorDataUpperBoundDisplayName")]
		public virtual T UpperBound
		{
			get { return (T)this[UpperBoundPropertyName]; }
			set { this[UpperBoundPropertyName] = value; }
		}

		private const string UpperBoundTypePropertyName = "upperBoundType";
		/// <summary>
		/// Gets or sets the upper bound type for the represented validator.
		/// </summary>
		[ConfigurationProperty(UpperBoundTypePropertyName, DefaultValue = RangeBoundaryType.Inclusive)]
        [ResourceDescription(typeof(DesignResources), "RangeValidatorDataUpperBoundTypeDescription")]
        [ResourceDisplayName(typeof(DesignResources), "RangeValidatorDataUpperBoundTypeDisplayName")]
		public RangeBoundaryType UpperBoundType
		{
			get { return (RangeBoundaryType)this[UpperBoundTypePropertyName]; }
			set { this[UpperBoundTypePropertyName] = value; }
		}
	}
}
