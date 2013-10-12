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
using System.Configuration;
using System.ComponentModel;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
    /// <summary>
	/// Represents the configuration settings that describe an <see cref="IConfigurationSource"/>.
	/// </summary>
    [Browsable(false)]
    [Command(ConfigurationSourcesDesignTime.CommandTypeNames.ConfigurationSourceElementDeleteCommand, 
        CommandPlacement = CommandPlacement.ContextDelete,
        Replace = CommandReplacement.DefaultDeleteCommandReplacement)]
    public class ConfigurationSourceElement : NameTypeConfigurationElement
    {
        /// <summary>
		/// Initializes a new instance of the <see cref="ConfigurationSourceElement"/> class with default values.
		/// </summary>
        public ConfigurationSourceElement() 
        {
        }

        /// <summary>
		/// Initializes a new instance of the <see cref="ConfigurationSourceElement"/> class with a name and an type.
		/// </summary>
        /// <param name="name">The instance name.</param>
		/// <param name="type">The type for the represented <see cref="IConfigurationSource"/>.</param>
        public ConfigurationSourceElement(string name, Type type)
            : base(name, type)
		{
		}

		/// <summary>
		/// Returns a new <see cref="IConfigurationSource"/> configured with the receiver's settings.
		/// </summary>
		/// <returns>A new configuration source.</returns>
		public virtual IConfigurationSource CreateSource()
		{
			throw new ConfigurationErrorsException(Resources.ExceptionBaseConfigurationSourceElementIsInvalid);
		}

        ///<summary>
        /// Returns a new <see cref="IDesignConfigurationSource"/> configured based on this configuration element.
        ///</summary>
        ///<returns>Returns a new <see cref="IDesignConfigurationSource"/> or null if this source does not have design-time support.</returns>
        public virtual IDesignConfigurationSource CreateDesignSource(IDesignConfigurationSource rootSource)
        {
            
            return null;
        }
    }
}
