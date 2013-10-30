using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate {

    public class RelativeDateTimeValidatorOperation : ValidationOperation {

        public RelativeDateTimeValidatorOperation(
            string keyToValidate,
            string outKey,
            int lowerBound,
            DateTimeUnit lowerUnit,
            RangeBoundaryType lowerBoundaryType,
            int upperBound,
            DateTimeUnit upperUnit,
            RangeBoundaryType upperBoundaryType,
            string messageTemplate,
            bool negated,
            bool append)
            : base(keyToValidate, outKey, append) {

            Validator = new RelativeDateTimeValidator(
                lowerBound,
                lowerUnit,
                lowerBoundaryType,
                upperBound,
                upperUnit,
                upperBoundaryType,
                messageTemplate,
                negated
            ) { Tag = keyToValidate };
        }
    }
}