using System.Text.RegularExpressions;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate
{
    public class RegexValidatorOperation : ValidationOperation {
        public RegexValidatorOperation(string inKey, string resultKey, string messageKey, string pattern, string message, bool negated, bool messageAppend)
            : base(inKey, resultKey, messageKey, messageAppend) {
            Validator = new RegexValidator(pattern, RegexOptions.Compiled, negated) { MessageTemplate = message, Tag = inKey };
            }
    }
}