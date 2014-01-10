using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate {

    public class RelativeDateTimeValidatorOperation : ValidationOperation {

        public RelativeDateTimeValidatorOperation(
            string keyToValidate,
            string resultKey,
            string messageKey,
            int lowerBound,
            DateTimeUnit lowerUnit,
            RangeBoundaryType lowerBoundaryType,
            int upperBound,
            DateTimeUnit upperUnit,
            RangeBoundaryType upperBoundaryType,
            string messageTemplate,
            bool negated,
            bool messageAppend)
            : base(keyToValidate, resultKey, messageKey, messageAppend) {

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