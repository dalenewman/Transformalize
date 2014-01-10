using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate {
    public class NotNullValidatorOperation : ValidationOperation {
        public NotNullValidatorOperation(string keyToValidate, string resultKey, string messageKey, string messageTemplate, bool negated, bool messageAppend)
            : base(keyToValidate, resultKey, messageKey, messageAppend) {
            Validator = new NotNullValidator(negated, messageTemplate) { Tag = keyToValidate };
        }
    }
}