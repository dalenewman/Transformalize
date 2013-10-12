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

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design
{
    /// <summary>
    /// Attribute class used to decorate the design time view model with a Add Application Block command. <br/>
    /// Add Application Block commands are added to the configuration tools main menu, underneath the 'Blocks' menu item.
    /// </summary>
    public class AddApplicationBlockCommandAttribute : CommandAttribute
    {
        private readonly string sectionName;
        private readonly Type configurationSectionType;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddApplicationBlockCommandAttribute"/> class.
        /// </summary>
        /// <param name="sectionName">The name of the configuration section that belongs to the application block that will be added.</param>
        /// <param name="configurationSectionType">The type of the configuration section that belongs to the application block that will be added.</param>
        public AddApplicationBlockCommandAttribute(string sectionName, Type configurationSectionType)
            : base(CommonDesignTime.CommandTypeNames.AddApplicationBlockCommand)
        {
            CommandPlacement = CommandPlacement.BlocksMenu;

            this.sectionName = sectionName;
            this.configurationSectionType = configurationSectionType;
        }

        /// <summary>
        /// Gets the name of the configuration section that belongs to the application block that will be added.
        /// </summary>
        public string SectionName
        {
            get { return sectionName; }
        }

        /// <summary>
        /// Gets the type of the configuration section that belongs to the application block that will be added.
        /// </summary>
        public Type ConfigurationSectionType
        {
            get { return configurationSectionType; }
        }
    }
}
