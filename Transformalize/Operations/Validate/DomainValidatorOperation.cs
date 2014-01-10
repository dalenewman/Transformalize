using System.Collections.Generic;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate {
    public class DomainValidatorOperation : ValidationOperation {

        public DomainValidatorOperation(string keyToValidate, string resultKey, string messageKey, IEnumerable<object> domain, string messageTemplate, bool negated, bool messageAppend)
            : base(keyToValidate, resultKey, messageKey, messageAppend) {

            Validator = new DomainValidator<object>(
                domain,
                messageTemplate,
                negated
            ) { Tag = keyToValidate };
        }

    }
}
