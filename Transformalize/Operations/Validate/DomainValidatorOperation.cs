using System.Collections.Generic;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate {
    public class DomainValidatorOperation : ValidationOperation {

        public DomainValidatorOperation(string keyToValidate, string resultKey, IEnumerable<object> domain, bool negated)
            : base(keyToValidate, resultKey) {

            Validator = new DomainValidator<object>(
                domain,
                negated
            ) { Tag = keyToValidate };
        }

    }
}
