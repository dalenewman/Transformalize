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
using System.ComponentModel;
using System.Globalization;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Configuration
{
	/// <summary>
	/// Describes a <see cref="RangeValidator"/>.
	/// </summary>
    [ResourceDisplayName(typeof(DesignResources), "RangeValidatorDataDisplayName")]
    [ResourceDescription(typeof(DesignResources), "RangeValidatorDataDescription")]
	public class RangeValidatorData : RangeValidatorData<string>
	{
		/// <summary>
		/// <para>Initializes a new instance of the <see cref="RangeValidatorData"/> class.</para>
		/// </summary>
		public RangeValidatorData() { Type = typeof(RangeValidator); }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="RangeValidatorData"/> class with a name.</para>
		/// </summary>
		public RangeValidatorData(string name)
			: base(name, typeof(RangeValidator))
		{ }

        private const string CulturePropertyName = "culture";

        /// <summary>
        /// Gets or sets the name of the culture that will be used to read lower and upperbound from the configuration file.
        /// </summary>
        [ConfigurationProperty(CulturePropertyName)]
        [TypeConverter(typeof(ConfigurationCultureInfoConverter))]
        [ViewModel(ValidationDesignTime.ViewModelTypeNames.RangeValidatorCultureProperty)]
        [ResourceDescription(typeof(DesignResources), "RangeValidatorDataCultureDescription")]
        [ResourceDisplayName(typeof(DesignResources), "RangeValidatorDataCultureDisplayName")]
        public CultureInfo Culture
        {
            get { return (CultureInfo)this[CulturePropertyName]; }
            set { this[CulturePropertyName] = value; }
        }

		/// <summary>
		/// Creates the <see cref="RangeValidator"/> described by the configuration object.
		/// </summary>
		/// <param name="targetType">The type of object that will be validated by the validator.</param>
		/// <returns>The created <see cref="TypeConversionValidator"/>.</returns>	
		protected override Validator DoCreateValidator(Type targetType)
		{
			TypeConverter typeConverter = null;
			IComparable lowerBound = null;
			IComparable upperBound = null;
			
			if (targetType != null)
			{
				typeConverter = TypeDescriptor.GetConverter(targetType);
				if (typeConverter != null)
				{
                    //backwards compatibility
                    var conversionCulture = Culture ?? CultureInfo.CurrentCulture;

                    lowerBound = (IComparable)typeConverter.ConvertFromString(null, conversionCulture, LowerBound);
                    upperBound = (IComparable)typeConverter.ConvertFromString(null, conversionCulture, UpperBound); 
				}
			}

			return new RangeValidator(lowerBound, LowerBoundType, upperBound, UpperBoundType, MessageTemplate, Negated);
		}
	}
}
