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
	internal static class ValidatorArgumentsValidatorHelper
	{
		internal static void ValidateContainsCharacterValidator(string characterSet)
		{
			if (characterSet == null)
			{
				throw new ArgumentNullException("characterSet");
			}
		}

		internal static void ValidateDomainValidator(object domain)
		{
			if (domain == null)
			{
				throw new ArgumentNullException("domain");
			}
		}

		internal static void ValidateEnumConversionValidator(Type enumType)
		{
			if (null == enumType)
			{
				throw new ArgumentNullException("enumType");
			}
		}

		internal static void ValidateRangeValidator(IComparable lowerBound, RangeBoundaryType lowerBoundaryType, IComparable upperBound, RangeBoundaryType upperBoundaryType)
		{
			if (lowerBoundaryType != RangeBoundaryType.Ignore && null == lowerBound)
			{
				throw new ArgumentNullException("lowerBound");
			}
			else if (upperBoundaryType != RangeBoundaryType.Ignore && null == upperBound)
			{
				throw new ArgumentNullException("upperBound");
			}
			else if (lowerBoundaryType == RangeBoundaryType.Ignore && upperBoundaryType == RangeBoundaryType.Ignore)
			{
				throw new ArgumentException(Resources.ExceptionCannotIgnoreBothBoundariesInRange, "lowerBound");
			}

			if (lowerBound != null && upperBound != null && lowerBound.GetType() != upperBound.GetType())
			{
				throw new ArgumentException(Resources.ExceptionTypeOfBoundsMustMatch, "upperBound");
			}
		}

		internal static void ValidateRegexValidator(string pattern, string patternResourceName, Type patternResourceType)
		{
			if (null == pattern && (patternResourceName == null || patternResourceType == null))
			{
				throw new ArgumentNullException("pattern");
			}
			else if (pattern == null && patternResourceName == null)
			{
				throw new ArgumentNullException("patternResourceName");
			}
			else if (pattern == null && patternResourceType == null)
			{
				throw new ArgumentNullException("patternResourceType");
			}
		}

		internal static void ValidateRelativeDatimeValidator(int lowerBound, DateTimeUnit lowerUnit, RangeBoundaryType lowerBoundType,
			int upperBound, DateTimeUnit upperUnit, RangeBoundaryType upperBoundType)
		{
			if ((lowerBound != 0 && lowerUnit == DateTimeUnit.None && lowerBoundType != RangeBoundaryType.Ignore) ||
				(upperBound != 0 && upperUnit == DateTimeUnit.None && upperBoundType != RangeBoundaryType.Ignore))
			{
				throw new ArgumentException(Resources.RelativeDateTimeValidatorNotValidDateTimeUnit);
			}
		}

		internal static void ValidateTypeConversionValidator(Type targetType)
		{
			if (null == targetType)
			{
				throw new ArgumentNullException("targetType");
			}
		}
	}
}
