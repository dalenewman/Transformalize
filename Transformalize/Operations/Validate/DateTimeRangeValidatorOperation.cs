using System;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate {
    public class DateTimeRangeValidatorOperation : ValidationOperation {

        public DateTimeRangeValidatorOperation(
            string keyToValidate,
            string resultKey,
            string messageKey,
            DateTime lowerBound,
            RangeBoundaryType lowerBoundary,
            DateTime upperBound,
            RangeBoundaryType upperBoundary,
            string messageTemplate,
            bool negated,
            bool messageAppend) : base(keyToValidate, resultKey, messageKey, messageAppend) {

            Validator = new DateTimeRangeValidator(
                lowerBound,
                lowerBoundary,
                upperBound,
                upperBoundary,
                messageTemplate,
                negated
            ) { Tag = keyToValidate };
        }
    }
}