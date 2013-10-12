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
using System.ComponentModel;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design
{
    ///<summary>
    /// Determines if the corresponding property is read-only at designtime.
    ///</summary>
    ///<remarks>
    /// This attribute is used to mark properties that should be presented as read-only, but underlying code may change the value on.
    /// <seealso cref="ReadOnlyAttribute"/></remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class DesignTimeReadOnlyAttribute : Attribute
    {
        ///<summary>
        /// Initializes a new instance of the <see cref="DesignTimeReadOnlyAttribute"/> class.
        ///</summary>
        ///<param name="readOnly"><see langword="true"/> if the property should be read-only at designtime.</param>
        public DesignTimeReadOnlyAttribute(bool readOnly)
        {
            ReadOnly = readOnly;
        }

        ///<summary>
        /// Determines if the property is read-only by design-time.
        /// Returns <see langword="true" /> if the property is read-only at design-time
        /// and <see langword="false" /> otherwise.
        ///</summary>
        public bool ReadOnly { get; private set; }

    }
}
