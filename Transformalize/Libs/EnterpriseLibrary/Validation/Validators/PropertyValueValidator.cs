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

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators
{
	/// <summary>
	/// Validates the value of a property using a configured validator.
	/// </summary>
	/// <typeparam name="T">The type for which validation on a property is to be performed.</typeparam>
	public class PropertyValueValidator<T> : MemberAccessValidator<T>
	{
		/// <summary>
		/// <para>Initializes a new instance of the <see cref="PropertyValueValidator{T}"/> class.</para>
		/// </summary>
		/// <param name="propertyName">The name of the property to validate.</param>
		/// <param name="propertyValueValidator">The validator for the value of the property.</param>
		public PropertyValueValidator(string propertyName, Validator propertyValueValidator)
			: base(GetPropertyValueAccess(propertyName), propertyValueValidator)
		{ }

		private static ValueAccess GetPropertyValueAccess(string propertyName)
		{
			return new PropertyValueAccess(ValidationReflectionHelper.GetProperty(typeof(T), propertyName, true));
		}
	}
}
