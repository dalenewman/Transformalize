using System;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate {
    public class DateTimeRangeValidatorOperation : ValidationOperation {

        public DateTimeRangeValidatorOperation(
            string keyToValidate,
            string resultKey,
            DateTime lowerBound,
            RangeBoundaryType lowerBoundary,
            DateTime upperBound,
            RangeBoundaryType upperBoundary,

            bool negated) : base(keyToValidate, resultKey) {

            Validator = new DateTimeRangeValidator(
                lowerBound,
                lowerBoundary,
                upperBound,
                upperBoundary,
                negated
            ) { Tag = keyToValidate };
        }
    }
}