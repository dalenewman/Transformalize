namespace Transformalize.Operations.Validate
{
    public class ParseJsonOperation : ValidationOperation {
        public ParseJsonOperation(string inKey, string outKey, bool append)
            : base(inKey, outKey, append) {
            Validator = new JsonValidator(inKey);
        }
    }
}