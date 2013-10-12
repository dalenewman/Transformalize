//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Core
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System.Configuration;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
	/// <summary>
	/// This interface must be implemented by configuration objects for custom providers that rely
	/// on a <see cref="CustomProviderDataHelper{T}"/> to perform the dynamic properties management.
	/// </summary>
	/// <remarks>
	/// This interface is generic so that the helper can be strongly-typed.
	/// </remarks>
	/// <typeparam name="T">The configuration object type. It must match the type implementing the interface.</typeparam>
	public interface IHelperAssistedCustomConfigurationData<T> : ICustomProviderData
		where T : NameTypeConfigurationElement, IHelperAssistedCustomConfigurationData<T>
	{
		/// <summary>
		/// Gets the helper that manages the configuration information.
		/// </summary>
		CustomProviderDataHelper<T> Helper { get; }

		/// <summary>
		/// This method supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
		/// Returns the value for the specified property using the inherited implementation.
		/// </summary>
		/// <param name="property">The property to get the value from.</param>
		/// <returns>The value for the property.</returns>
		object BaseGetPropertyValue(ConfigurationProperty property);

		/// <summary>
		/// This method supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
		/// Returns the modification status using the inherited implementation.
		/// </summary>
		/// <returns><b>true</b> if the configuration element has been modified, <b>false</b> otherwise.</returns>
		bool BaseIsModified();

		/// <summary>
		/// This method supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
		/// Resets the internal state of the configuration object using the inherited implementation.
		/// </summary>
		/// <param name="parentElement">The parent node of the configuration element.</param>
		void BaseReset(ConfigurationElement parentElement);

		/// <summary>
		/// This method supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
		/// Sets the value for the specified property using the inherited implementation.
		/// </summary>
		/// <param name="property">The property to set the value to.</param>
		/// <param name="value">The new value for the property.</param>
		void BaseSetPropertyValue(ConfigurationProperty property, object value);

		/// <summary>
		/// This method supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
		/// Modifies the <b>ConfigurationElement</b> object to remove all values that should not be saved using the inherited implementation.
		/// </summary>
		/// <param name="sourceElement">A <see cref="ConfigurationElement"/> object at the current level containing a merged view of the properties.</param>
		/// <param name="parentElement">The parent <b>ConfigurationElement</b> object, or a null reference (Nothing in Visual Basic) if this is the top level.</param>
		/// <param name="saveMode">A <see cref="ConfigurationSaveMode"/> object that determines which property values to include.</param>
		void BaseUnmerge(ConfigurationElement sourceElement, ConfigurationElement parentElement, ConfigurationSaveMode saveMode);
	}
}
