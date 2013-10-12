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
    /// Interface used to implement custom clone behavior for a <see cref="ConfigurationElement"/>.
    /// </summary>
    /// <seealso cref="ConfigurationSectionCloner"/>
    public interface ICloneableConfigurationElement
    {
        /// <summary>
        /// Creates a deep clone of the current <see cref="ConfigurationElement"/> instance.
        /// </summary>
        /// <returns>
        /// A deep clone of the current <see cref="ConfigurationElement"/> instance.
        /// </returns>
        ConfigurationElement CreateFullClone();
    }
}
