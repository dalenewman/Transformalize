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

using System.ComponentModel;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
    /// <summary>
    /// Type converter used to work around enums with enums wrongly marked as "flags".
    /// </summary>
    public class EnumConverterWithExclusiveStandardValues<T> : EnumConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumConverterWithExclusiveStandardValues{T}"/> class.
        /// </summary>
        public EnumConverterWithExclusiveStandardValues()
            : base(typeof(T))
        { }

        /// <summary>
        /// Indicates where the standard values are exclusive.
        /// </summary>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
