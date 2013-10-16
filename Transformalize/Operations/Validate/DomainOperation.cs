using System.Collections.Generic;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate {
    public class DomainOperation : ValidationOperation {

        public DomainOperation(string inKey, string outKey, IEnumerable<object> domain, string messageTemplate, bool negated, bool append)
            : base(inKey, outKey, append) {

            Validator = new DomainValidator<object>(
                domain,
                messageTemplate,
                negated
            ) { Tag = inKey };
        }

    }
}
