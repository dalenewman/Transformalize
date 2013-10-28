using System;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate {
    public class DateTimeRangeOperation : ValidationOperation {

        public DateTimeRangeOperation(string keyToValidate, string outKey, DateTime lowerBound, string lowerBoundary, DateTime upperBound, string upperBoundary, string messageTemplate, bool negated, bool append)
            : base(keyToValidate, outKey, append) {
            Validator = new DateTimeRangeValidator(
                lowerBound,
                (RangeBoundaryType)Enum.Parse(typeof(RangeBoundaryType), lowerBoundary, true),
                upperBound,
                (RangeBoundaryType)Enum.Parse(typeof(RangeBoundaryType), upperBoundary, true),
                messageTemplate,
                negated
            ) { Tag = keyToValidate };
        }
    }
}