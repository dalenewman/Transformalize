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
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Configuration
{
	/// <summary>
	/// Configuration object to describe an instance of class <see cref="TypeConversionValidatorData"/>.
	/// </summary>
    [ResourceDescription(typeof(DesignResources), "TypeConversionValidatorDataDescription")]
    [ResourceDisplayName(typeof(DesignResources), "TypeConversionValidatorDataDisplayName")]
	public class TypeConversionValidatorData : ValueValidatorData
	{
		private static readonly AssemblyQualifiedTypeNameConverter typeConverter = new AssemblyQualifiedTypeNameConverter();

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="TypeConversionValidatorData"/> class.</para>
		/// </summary>
        public TypeConversionValidatorData() { Type = typeof(TypeConversionValidator); }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="TypeConversionValidatorData"/> class with a name.</para>
		/// </summary>
		/// <param name="name">The name for the instance.</param>
		public TypeConversionValidatorData(string name)
			: base(name, typeof(TypeConversionValidator))
		{ }

		private const string TargetTypeNamePropertyName = "targetType";
		/// <summary>
		/// Gets or sets name of the type the represented <see cref="TypeConversionValidator"/> must use for testing conversion.
		/// </summary>
		[ConfigurationProperty(TargetTypeNamePropertyName, IsRequired=true)]
        [Editor(CommonDesignTime.EditorTypes.TypeSelector, CommonDesignTime.EditorTypes.UITypeEditor)]
        [BaseType(typeof(object))]
        [ResourceDescription(typeof(DesignResources), "TypeConversionValidatorDataTargetTypeNameDescription")]
        [ResourceDisplayName(typeof(DesignResources), "TypeConversionValidatorDataTargetTypeNameDisplayName")]
		public string TargetTypeName
		{
			get { return (string)this[TargetTypeNamePropertyName]; }
			set { this[TargetTypeNamePropertyName] = value; }
		}

		/// <summary>
		/// Gets or sets the target element type.
		/// </summary>
		public Type TargetType
		{
			get { return (Type)typeConverter.ConvertFrom(TargetTypeName); }
			set { TargetTypeName = typeConverter.ConvertToString(value); }
		}

		/// <summary>
		/// Creates the <see cref="TypeConversionValidator"/> described by the configuration object.
		/// </summary>
		/// <param name="targetType">The type of object that will be validated by the validator.</param>
		/// <returns>The created <see cref="TypeConversionValidator"/>.</returns>	
		protected override Validator DoCreateValidator(Type targetType)
		{
			return new TypeConversionValidator(TargetType, Negated);
		}
	}
}
