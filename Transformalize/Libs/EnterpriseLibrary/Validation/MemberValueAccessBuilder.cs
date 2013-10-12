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
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
	/// <summary>
	/// Creates <see cref="ValueAccess"/> objects for member descriptors.
	/// </summary>
	/// <remarks>
	/// Subclasses might return <see cref="ValueAccess"/> implementations that do not assume
	/// a value is being extracted from the type that owns the member descriptors.<para/>
	/// This is necessary in the integration scenarios.
	/// </remarks>
	public abstract class MemberValueAccessBuilder
	{
		/// <summary>
		/// Creates a <see cref="ValueAccess"/> suitable for accessing the value in <paramref name="fieldInfo"/>.
		/// </summary>
		/// <param name="fieldInfo">The <see cref="FieldInfo"/></param> representing the field for which a
		/// <see cref="ValueAccess"/> is required.
		/// <returns>The <see cref="ValueAccess"/> that allows for accessing the value in <paramref name="fieldInfo"/>.</returns>
		/// <exception cref="ArgumentNullException">when <paramref name="fieldInfo"/> is <see langword="null"/>.</exception>
		public ValueAccess GetFieldValueAccess(FieldInfo fieldInfo)
		{
			if (null == fieldInfo)
				throw new ArgumentNullException("fieldInfo");

			return DoGetFieldValueAccess(fieldInfo);
		}

		/// <summary>
		/// Performs the actual creation of a <see cref="ValueAccess"/> suitable for accessing the value in <paramref name="fieldInfo"/>.
		/// </summary>
		/// <param name="fieldInfo">The <see cref="FieldInfo"/></param> representing the field for which a
		/// <see cref="ValueAccess"/> is required.
		/// <returns>The <see cref="ValueAccess"/> that allows for accessing the value in <paramref name="fieldInfo"/>.</returns>
		protected abstract ValueAccess DoGetFieldValueAccess(FieldInfo fieldInfo);

		/// <summary>
		/// Creates a <see cref="ValueAccess"/> suitable for accessing the return value for <paramref name="methodInfo"/>.
		/// </summary>
		/// <param name="methodInfo">The <see cref="MethodInfo"/></param> representing the method for which a
		/// <see cref="ValueAccess"/> is required.
		/// <returns>The <see cref="ValueAccess"/> that allows for accessing the value in <paramref name="methodInfo"/>.</returns>
		/// <exception cref="ArgumentNullException">when <paramref name="methodInfo"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">when <paramref name="methodInfo"/> represents a method with a non-void return type.</exception>
		/// <exception cref="ArgumentException">when <paramref name="methodInfo"/> represents a method with parameters.</exception>
		public ValueAccess GetMethodValueAccess(MethodInfo methodInfo)
		{
			if (null == methodInfo)
				throw new ArgumentNullException("methodInfo");
			if (typeof(void) == methodInfo.ReturnType)
				throw new ArgumentException(Resources.ExceptionMethodHasNoReturnValue, "methodInfo");
			if (0 < methodInfo.GetParameters().Length)
				throw new ArgumentException(Resources.ExceptionMethodHasParameters, "methodInfo");

			return DoGetMethodValueAccess(methodInfo);
		}

		/// <summary>
		/// Performs the actual creation of a <see cref="ValueAccess"/> suitable for accessing the value in <paramref name="methodInfo"/>.
		/// </summary>
		/// <param name="methodInfo">The <see cref="MethodInfo"/></param> representing the method for which a
		/// <see cref="ValueAccess"/> is required.
		/// <returns>The <see cref="ValueAccess"/> that allows for accessing the value in <paramref name="methodInfo"/>.</returns>
		protected abstract ValueAccess DoGetMethodValueAccess(MethodInfo methodInfo);

		/// <summary>
		/// Creates a <see cref="ValueAccess"/> suitable for accessing the value for <paramref name="propertyInfo"/>.
		/// </summary>
		/// <param name="propertyInfo">The <see cref="PropertyInfo"/></param> representing the property for which a
		/// <see cref="ValueAccess"/> is required.
		/// <returns>The <see cref="ValueAccess"/> that allows for accessing the value in <paramref name="propertyInfo"/>.</returns>
		/// <exception cref="ArgumentNullException">when <paramref name="propertyInfo"/> is <see langword="null"/>.</exception>
		public ValueAccess GetPropertyValueAccess(PropertyInfo propertyInfo)
		{
			if (null == propertyInfo)
				throw new ArgumentNullException("propertyInfo");

			return DoGetPropertyValueAccess(propertyInfo);
		}

		/// <summary>
		/// Performs the actual creation of a <see cref="ValueAccess"/> suitable for accessing the value in <paramref name="propertyInfo"/>.
		/// </summary>
		/// <param name="propertyInfo">The <see cref="PropertyInfo"/></param> representing the property for which a
		/// <see cref="ValueAccess"/> is required.
		/// <returns>The <see cref="ValueAccess"/> that allows for accessing the value in <paramref name="propertyInfo"/>.</returns>
		protected abstract ValueAccess DoGetPropertyValueAccess(PropertyInfo propertyInfo);
	}
}
