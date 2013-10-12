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
    /// Attribute class that allows to specify a property that should be used as the Element View Model's name.<br/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class NamePropertyAttribute : Attribute
    {
        private readonly string propertyName;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamePropertyAttribute"/> class.
        /// </summary>
        /// <param name="propertyName">The reflection name of the property that will be used as the Element View Model's name.</param>
        public NamePropertyAttribute(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, "propertyName");
            
            this.propertyName =  propertyName;
            this.NamePropertyDisplayFormat = "{0}";
        }

        /// <summary>
        /// Gets the reflection name of the property that will be used as the Element View Model's name.
        /// </summary>
        public string PropertyName
        {
            get { return propertyName; }
        }

        /// <summary>
        /// Gets the Display Format that will be used to display the name property.<br/>
        /// The Display Format should be a Format-string with 1 argument:<Br/>
        /// The token '{0}' will be replaced with the Name Properties value.
        /// </summary>
        public string NamePropertyDisplayFormat { get; set; }
    }
}
