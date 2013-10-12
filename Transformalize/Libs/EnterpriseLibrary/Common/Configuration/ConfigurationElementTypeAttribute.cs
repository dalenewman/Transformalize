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

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
	/// <summary>
	/// Indicates the configuration object type that is used for the attributed object.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class ConfigurationElementTypeAttribute : Attribute
	{
		//private Type configurationType;
	    private string typeName;

	    /// <summary>
		/// Initialize a new instance of the <see cref="EnterpriseLibrary.Common.Configuration.ConfigurationElementTypeAttribute"/> class.
		/// </summary>
		public ConfigurationElementTypeAttribute()
		{
		}

		/// <summary>
		/// Initialize a new instance of the <see cref="ConfigurationElementTypeAttribute"/> class with the configuration object type.
		/// </summary>
		/// <param name="configurationType">The <see cref="Type"/> of the configuration object.</param>
		public ConfigurationElementTypeAttribute(Type configurationType)
            : this(configurationType == null ? null : configurationType.AssemblyQualifiedName)
		{
		}

        /// <summary>
        /// Initialize a new instance of the <see cref="ConfigurationElementTypeAttribute"/> class with the configuration object type.
        /// </summary>
        /// <param name="typeName">The <see cref="Type"/> name of the configuration object.</param>
        public ConfigurationElementTypeAttribute(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, "typeName");
            this.typeName = typeName;
        }

		/// <summary>
		/// Gets the <see cref="Type"/> of the configuration object.
		/// </summary>
		/// <value>
		/// The <see cref="Type"/> of the configuration object.
		/// </value>
		public Type ConfigurationType
		{
            get { return Type.GetType(TypeName); }			
		}

        /// <summary>
        /// Gets <see cref="Type"/> name of the configuration object.
        /// </summary>
	    public string TypeName
	    {
            get { return typeName; }            
	    }
	}
}
