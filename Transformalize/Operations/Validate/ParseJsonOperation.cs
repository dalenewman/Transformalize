namespace Transformalize.Operations.Validate
{
    public class ParseJsonOperation : ValidationOperation {
        public ParseJsonOperation(string keyToValidate, string outKey, bool append)
            : base(keyToValidate, outKey, append) {
            Validator = new JsonValidator(keyToValidate);
        }
    }
}