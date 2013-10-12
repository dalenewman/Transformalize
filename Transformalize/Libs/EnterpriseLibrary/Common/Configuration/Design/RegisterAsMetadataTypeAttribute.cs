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
    /// Registers a class as a metadata body class for another class.
    /// </summary>
    /// <remarks>
    /// When applying metadata attributes to classes, the target class might not always allow itself to be anotated. <br/>
    /// This attribute can be used to nominate another class to contain the metadata attributes. <br/>
    /// The metadata type should follow the same structure as the target type and its members cab be decorated with the metadata attributes.<br/>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterAsMetadataTypeAttribute : Attribute
    {
        private readonly Type targetType;

        /// <summary>
        /// Creates a new instance of <see cref="RegisterAsMetadataTypeAttribute"/>.
        /// </summary>
        /// <param name="targetType">The type for which this class should contain metadata attributes.</param>
        public RegisterAsMetadataTypeAttribute(Type targetType)
        {
            this.targetType = targetType;
        }

        /// <summary>
        /// Gets the type for which this class should contain metadata attributes.
        /// </summary>
        /// <value>
        /// The type for which this class should contain metadata attributes.
        /// </value>
        public Type TargetType
        {
            get { return targetType; }
        }
    }
}
