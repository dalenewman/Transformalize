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
	/// Event handler called after a configuration has changed.
    /// </summary>
    /// <param name="sender">
    /// <para>The source of the event.</para>
    /// </param>
    /// <param name="e">
    /// <para>A <see cref="ConfigurationChangedEventArgs"/> that contains the event data.</para>
    /// </param>
    public delegate void ConfigurationChangedEventHandler(object sender, ConfigurationChangedEventArgs e);

    /// <summary>
    /// </summary>
    [Serializable]
    public class ConfigurationChangedEventArgs : EventArgs
    {
        private readonly string sectionName;

        /// <summary>
        /// <para>Initialize a new instance of the <see cref="ConfigurationChangedEventArgs"/> class with the section name</para>
        /// </summary>
        /// <param name="sectionName"><para>The section name of the changes.</para></param>
        public ConfigurationChangedEventArgs(string sectionName)
        {
            this.sectionName = sectionName;
        }

        /// <summary>
        /// <para>Gets the section name where the changes occurred.</para>
        /// </summary>
        /// <value>
        /// <para>The section name where the changes occurred.</para>
        /// </value>
        public string SectionName
        {
            get { return sectionName; }
        }
    }
    
}
