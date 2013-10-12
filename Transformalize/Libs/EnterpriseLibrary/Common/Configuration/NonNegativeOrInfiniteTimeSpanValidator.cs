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
using System.Configuration;
using System.Threading;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
    /// <summary>
    /// Provides validation for a <see cref="TimeSpan"/> object allowing non-negative spans and 
    /// the value for <see cref="Timeout.InfiniteTimeSpan"/>.
    /// </summary>
    public class NonNegativeOrInfiniteTimeSpanValidator : TimeSpanValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonNegativeOrInfiniteTimeSpanValidator"/> class.
        /// </summary>
        public NonNegativeOrInfiniteTimeSpanValidator()
            : base(TimeSpan.Zero, TimeSpan.MaxValue)
        {
        }

        /// <summary>
        /// Determines whether the value of an object is valid.
        /// </summary>
        /// <param name="value">The value of an object.</param>
        public override void Validate(object value)
        {
            if (!(value is TimeSpan && ((TimeSpan)value) == Timeout.InfiniteTimeSpan))
            {
                base.Validate(value);
            }
        }
    }
}
