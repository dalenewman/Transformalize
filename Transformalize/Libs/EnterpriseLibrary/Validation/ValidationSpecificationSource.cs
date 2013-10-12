//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Validation Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.ComponentModel;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration;

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
    /// <summary>
    /// Specifies the required source for validation information when invoking <see cref="Validator"/> creation methods.
    /// </summary>
    /// <seealso cref="ValidationFactory.CreateValidator{T}(string, IConfigurationSource)"/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames"), Flags]
    public enum ValidationSpecificationSource
    {
        /// <summary>
        /// Validation information is to be retrieved from attributes.
        /// </summary>
        Attributes = 1,

        /// <summary>
        /// Validation information is to be retrieved from configuration.
        /// </summary>
        Configuration = 2,

        /// <summary>
        /// Validation information is to be retrieved from both attributes and configuration.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        Both = 3,

        /// <summary>
        /// Validation information is to be retrieved from Data Annotations Validation Attributes.
        /// </summary>
        DataAnnotations = 4,

        /// <summary>
        /// Validation information is to be retrieved from all possible sources.
        /// </summary>
        All = 7
    }

    internal static class ValidationSpecificationSourceExtension
    {
        public static bool IsSet(this ValidationSpecificationSource source, ValidationSpecificationSource value)
        {
            return value == (source & value);
        }
    }
}
