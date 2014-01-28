namespace Transformalize.Operations.Validate
{
    public class JsonValidatorOperation : ValidationOperation {
        public JsonValidatorOperation(string keyToValidate, string resultKey, string messageKey, string messageTemplate, bool negated, bool messageAppend)
            : base(keyToValidate, resultKey, messageKey, messageAppend) {
            Validator = new JsonValidator(messageTemplate, keyToValidate, negated);
        }
    }
}