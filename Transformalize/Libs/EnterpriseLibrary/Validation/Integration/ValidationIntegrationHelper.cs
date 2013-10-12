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
using System.Reflection;
using System.ComponentModel;
using System.Globalization;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Integration
{
	/// <summary>
	/// Encapsulates the logic used to link validation for integration scenarios.
	/// </summary>
	public class ValidationIntegrationHelper
	{
		private IValidationIntegrationProxy integrationProxy;
		private Type validatedType;
		private PropertyInfo validatedProperty;

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationIntegrationHelper"/> for an <see cref="IValidationIntegrationProxy"/>.
		/// </summary>
		/// <exception cref="ArgumentNullException">when <paramref name="integrationProxy"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">when configuration is not complete.</exception>
		public ValidationIntegrationHelper(IValidationIntegrationProxy integrationProxy)
		{
			if (integrationProxy == null)
			{
				throw new ArgumentNullException("integrationProxy");
			}

			validatedType = integrationProxy.ValidatedType;
			if (validatedType == null)
			{
				throw new InvalidOperationException(Resources.ExceptionIntegrationValidatedTypeNotAvailable);
			}

			string validatedPropertyName = integrationProxy.ValidatedPropertyName;
			if (string.IsNullOrEmpty(validatedPropertyName))
			{
				throw new InvalidOperationException(Resources.ExceptionIntegrationValidatedPropertyNameNotAvailable);
			}

			this.validatedProperty = validatedType.GetProperty(validatedPropertyName, BindingFlags.Public | BindingFlags.Instance);
			if (this.validatedProperty == null)
			{
				throw new InvalidOperationException(Resources.ExceptionIntegrationValidatedPropertyNotExists);
			}
			if (!this.validatedProperty.CanRead)
			{
				throw new InvalidOperationException(Resources.ExceptionIntegrationValidatedPropertyNotReadable);
			}

			this.integrationProxy = integrationProxy;
		}

		/// <summary>
		/// Returns the <see cref="Validator"/> represented by the configuration in the <see cref="IValidationIntegrationProxy"/>, linked
		/// with the integration scenario as necessary.
		/// </summary>
		public Validator GetValidator()
		{
			return PropertyValidationFactory.GetPropertyValidator(validatedType,
				validatedProperty,
				integrationProxy.Ruleset,
				integrationProxy.SpecificationSource,
				integrationProxy.GetMemberValueAccessBuilder());
		}

		/// <summary>
		/// Returns the value to validate.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007",
			Justification = "Generics are not appropriate here")]
		public bool GetValue(out object value, out string valueRetrievalFailureMessage)
		{
			value = null;

			value = this.integrationProxy.GetRawValue();
			PerformValueConversion(ref value, out valueRetrievalFailureMessage);

			return valueRetrievalFailureMessage == null;
		}

		private void PerformValueConversion(ref object value, out string valueRetrievalFailureMessage)
		{
			valueRetrievalFailureMessage = null;

			if (this.integrationProxy.ProvidesCustomValueConversion)
			{
				ValueConvertEventArgs eventArgs = new ValueConvertEventArgs(value,
					this.validatedProperty.PropertyType,
					this.integrationProxy,
					this.validatedProperty.Name);
				this.integrationProxy.PerformCustomValueConversion(eventArgs);

				if (eventArgs.ConversionErrorMessage == null)
				{
					value = eventArgs.ConvertedValue;
				}
				else
				{
					value = null;
					valueRetrievalFailureMessage = eventArgs.ConversionErrorMessage;
				}
			}
			else
			{
				if (value != null)
				{
					if (value.GetType() == this.validatedProperty.PropertyType)
					{
						return;
					}

					try
					{
						TypeConverter converter = TypeDescriptor.GetConverter(this.validatedProperty.PropertyType);
						value = converter.ConvertFrom(null, CultureInfo.CurrentCulture, value);
					}
					catch (Exception e)
					{
						if (e.InnerException is FormatException)
						{
							value = null;
							valueRetrievalFailureMessage = string.Format(
								CultureInfo.CurrentCulture,
								Resources.ErrorCannotPerfomDefaultConversion,
								value,
								this.validatedProperty.PropertyType.FullName);
						}
						else
						{
							throw;
						}
					}
				}
			}

		}
	}
}
