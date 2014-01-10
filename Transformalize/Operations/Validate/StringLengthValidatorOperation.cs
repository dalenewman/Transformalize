using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate
{
    public class StringLengthValidatorOperation : ValidationOperation {

        public StringLengthValidatorOperation(

            string keyToValidate,
            string resultKey,
            string messageKey,
            int lowerBound,
            RangeBoundaryType lowerBoundaryType,
            int upperBound,
            RangeBoundaryType upperBoundaryType,
            string messageTemplate,
            bool negated,
            bool messageAppend)
            : base(keyToValidate, resultKey, messageKey, messageAppend) {
            
            Validator = new StringLengthValidator(
                lowerBound,
                lowerBoundaryType,
                upperBound,
                upperBoundaryType,
                messageTemplate,
                negated
                );
            }
    }
}