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
    /// Specifies a default value for a configuration property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class DesigntimeDefaultAttribute : Attribute
    {
        readonly string bindableDefaultValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="DesigntimeDefaultAttribute"/> class.
        /// </summary>
        /// <remarks>
        /// The default value is a string representation which will be converted using <see cref="System.Globalization.CultureInfo.InvariantCulture"/>.
        /// </remarks>
        /// <param name="bindableDefaultValue">The string representation of the default value.</param>
        public DesigntimeDefaultAttribute(string bindableDefaultValue)
        {
            this.bindableDefaultValue = bindableDefaultValue;
        }

        /// <summary>
        /// Gets the string reprentation of the default value.
        /// </summary>
        /// <value>
        /// The string reprentation of the default value.
        /// </value>
        public string BindableDefaultValue
        {
            get { return bindableDefaultValue; }
        }
    }
}
