using System;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate {
    public class TypeConversionValidatorOperation : ValidationOperation {
        public TypeConversionValidatorOperation(
            string inKey,
            string resultKey,
            Type targetType,
            bool negated,
            bool ignoreEmpty)
            : base(inKey, resultKey, ignoreEmpty) {
            Validator = new TypeConversionValidator(targetType, string.Empty, negated) { Tag = inKey };
        }
    }
}