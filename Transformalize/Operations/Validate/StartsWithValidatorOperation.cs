namespace Transformalize.Operations.Validate
{
    public class StartsWithValidatorOperation : ValidationOperation {
        public StartsWithValidatorOperation(string keyToValidate, string value, string resultKey, string messageKey, string messageTemplate, bool negated, bool messageAppend)
            : base(keyToValidate, resultKey, messageKey, messageAppend) {
            Validator = new StartsWithValidator(value, messageTemplate, keyToValidate, negated);
            }
    }
}