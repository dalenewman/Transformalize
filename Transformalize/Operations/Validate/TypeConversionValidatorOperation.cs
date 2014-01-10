using System;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate {
    public class TypeConversionValidatorOperation : ValidationOperation {
        public TypeConversionValidatorOperation(string inKey, string resultKey, string messageKey, Type targetType, string messageTemplate, bool negated, bool messageAppend)
            : base(inKey, resultKey, messageKey, messageAppend) {
            Validator = new TypeConversionValidator(targetType, messageTemplate, negated) { Tag = inKey };
        }
    }
}