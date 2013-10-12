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
    /// Provides attributes for the filter of types.
    /// </summary>
    [Flags]
    public enum TypeSelectorIncludes
    {
        /// <summary>
        /// No filter are applied to types.
        /// </summary>
        None = 0x00,
        /// <summary>
        /// Inclue abstract types in the filter.
        /// </summary>
        AbstractTypes = 0x01,
        /// <summary>
        /// Inclue interfaces in the filter.
        /// </summary>
        Interfaces = 0x02,
        /// <summary>
        /// Inclue base types in the filter.
        /// </summary>
        BaseType = 0x04,
        /// <summary>
        /// Inclue non public types in the filter.
        /// </summary>
        NonpublicTypes = 0x08,
        /// <summary>
        /// Include all types in the filter.
        /// </summary>
        All = 0x0F
    }
}
