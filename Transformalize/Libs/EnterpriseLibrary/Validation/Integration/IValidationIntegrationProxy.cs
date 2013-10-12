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
    /// Represents the behavior necessary to provide integration with the Validation Application Block.
	/// </summary>
	public interface IValidationIntegrationProxy
	{
		/// <summary>
		/// Performs a custom conversion of a value to validate.
		/// </summary>
		/// <param name="e">The <see cref="ValueConvertEventArgs"/> containing the value to convert.</param>
		/// <remarks>
		/// Implementors should set the <see cref="ValueConvertEventArgs.ConvertedValue"/> with the conversion result,
		/// and set the <see cref="ValueConvertEventArgs.ConversionErrorMessage"/> should an error occur during conversion.
		/// </remarks>
		void PerformCustomValueConversion(ValueConvertEventArgs e);

		/// <summary>
		/// Retrieves the raw value to validate, before conversion.
		/// </summary>
		object GetRawValue();

		/// <summary>
		/// Gets the <see cref="MemberValueAccessBuilder"/> to use when building a <see cref="Validator"/> in an integration scenario.
		/// </summary>
		MemberValueAccessBuilder GetMemberValueAccessBuilder();

		/// <summary>
		/// Gets the ruleset to use when building validators in an integration scenario.
		/// </summary>
		String Ruleset { get; }

		/// <summary>
		/// Gets the <see cref="ValidationSpecificationSource"/> to use when building validators in an integration scenario.
		/// </summary>
		ValidationSpecificationSource SpecificationSource { get; }

		/// <summary>
		/// Gets the indication of whether custom value conversion should be performed during validation.
		/// </summary>
		bool ProvidesCustomValueConversion { get; }

		/// <summary>
		/// Gets the name of the property to use when building validators in an integration scenario.
		/// </summary>
		String ValidatedPropertyName { get; }

		/// <summary>
		/// Gets the type to use when building validators in an integration scenario.
		/// </summary>
		Type ValidatedType { get; }
	}
}
