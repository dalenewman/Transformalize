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

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class RelativeDateTimeGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bound"></param>
        /// <param name="unit"></param>
        /// <param name="referenceDateTime"></param>
        /// <returns></returns>
        public DateTime GenerateBoundDateTime(int bound, DateTimeUnit unit, DateTime referenceDateTime)
        {
            DateTime result;

            switch (unit)
            {
                case DateTimeUnit.Day: result = referenceDateTime.AddDays(bound); break;
                case DateTimeUnit.Hour: result = referenceDateTime.AddHours(bound); break;
                case DateTimeUnit.Minute: result = referenceDateTime.AddMinutes(bound); break;
                case DateTimeUnit.Month: result = referenceDateTime.AddMonths(bound); break;
                case DateTimeUnit.Second: result = referenceDateTime.AddSeconds(bound); break;
                case DateTimeUnit.Year: result = referenceDateTime.AddYears(bound); break;
                default: result = referenceDateTime; break;
            }
            return result;
        }
    }
}
