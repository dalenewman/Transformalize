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

using System;
using System.Collections.Generic;
using System.Configuration;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
    /// <summary>
    /// Interface that allows a <see cref="ConfigurationElementCollection"/> to be merged.
    /// </summary>
    public interface IMergeableConfigurationElementCollection
    {
        /// <summary>
        /// Resets the elements in the <see cref="ConfigurationElementCollection"/> to the <see cref="ConfigurationElement"/>s passed as <paramref name="configurationElements" />.
        /// </summary>
        /// <param name="configurationElements">The new contents of this <see cref="ConfigurationElementCollection"/>.</param>
        void ResetCollection(IEnumerable<ConfigurationElement> configurationElements);

        /// <summary>
        /// Creates a new <see cref="ConfigurationElement"/> for the specifies <paramref name="configurationType" />.
        /// </summary>
        /// <param name="configurationType">The type of <see cref="ConfigurationElement"/> that should be created.</param>
        ConfigurationElement CreateNewElement(Type configurationType);
    }


}
