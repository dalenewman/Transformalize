using System;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate
{
    public class RangeValidatorOperation : ValidationOperation {
        public RangeValidatorOperation(
            string keyToValidate,
            string resultKey,
            IComparable lowerBound,
            RangeBoundaryType lowerBoundary,
            IComparable upperBound,
            RangeBoundaryType upperBoundary,
            bool negated)
            : base(keyToValidate, resultKey) {

            Validator = new RangeValidator(
                lowerBound,
                lowerBoundary,
                upperBound,
                upperBoundary,
                string.Empty,
                negated
                ) { Tag = keyToValidate };

            }
    }
}