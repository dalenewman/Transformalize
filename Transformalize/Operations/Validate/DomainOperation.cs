using System.Collections.Generic;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate {
    public class DomainOperation : ValidationOperation {

        public DomainOperation(string keyToValidate, string outKey, IEnumerable<object> domain, string messageTemplate, bool negated, bool append)
            : base(keyToValidate, outKey, append) {

            Validator = new DomainValidator<object>(
                domain,
                messageTemplate,
                negated
            ) { Tag = keyToValidate };
        }

    }
}
