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

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Integration
{
	/// <summary>
	/// Provides data to perform custom value conversions in validation integration scenarios.
	/// </summary>
	public sealed class ValueConvertEventArgs : EventArgs
	{
		private object valueToConvert;
		private Type targetType;
		private object valueSource;
		private string sourcePropertyName;
		private object convertedValue;

		/// <summary>
		/// Initializes a new instance of the <see cref="ValueConvertEventArgs"/> class.
		/// </summary>
		/// <param name="valueToConvert">The raw value to convert.</param>
		/// <param name="targetType">The type to which the <paramref name="valueToConvert"/> needs to be converted to.</param>
		/// <param name="valueSource">The object from which the value was extracted.</param>
		/// <param name="sourcePropertyName">The name of the property </param>
		public ValueConvertEventArgs(object valueToConvert, Type targetType, object valueSource, string sourcePropertyName)
		{
			this.valueToConvert = valueToConvert;
			this.targetType = targetType;
			this.valueSource = valueSource;
			this.sourcePropertyName = sourcePropertyName;
			this.convertedValue = valueToConvert;
		}

		/// <summary>
		/// Gets the value to convert, as retrieved from the control.
		/// </summary>
		public object ValueToConvert
		{
			get { return valueToConvert; }
		}

		/// <summary>
		/// Gets the type to which the value should be converted.
		/// </summary>
		public Type TargetType
		{
			get { return targetType; }
		}

		/// <summary>
		/// Gets the object for which the validation requiring the value conversion is being performed.
		/// </summary>
		public object ValueSource
		{
			get { return valueSource; }
		}

		/// <summary>
		/// Gets the name of the property that maps to the control that supplied the value to convert.
		/// </summary>
		public string SourcePropertyName
		{
			get { return sourcePropertyName; }
		}

		/// <summary>
		/// Gets or sets the converted value.
		/// </summary>
		/// <remarks>
		/// This property must be set to the desired value; otherwise 
		/// the <see cref="ValueConvertEventArgs.ValueToConvert"/> will be used.
		/// </remarks>
		public object ConvertedValue
		{
			get { return convertedValue; }
			set { convertedValue = value; }
		}

		private string conversionErrorMessage;
		/// <summary>
		/// Gets or sets the message describing the conversion failure.
		/// </summary>
		/// <remarks>
		/// The error message should be set to <see langword="null"/> if conversion was successful.
		/// </remarks>
		public string ConversionErrorMessage
		{
			get { return conversionErrorMessage; }
			set { conversionErrorMessage = value; }
		}
	}
}
