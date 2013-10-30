using System;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate
{
    public class TypeConversionValidatorOperation : ValidationOperation {
        public TypeConversionValidatorOperation(string inKey, string outKey, Type targetType, string messageTemplate, bool negated, bool append)
            : base(inKey, outKey, append) {
            Validator = new TypeConversionValidator(targetType, messageTemplate, negated) { Tag = inKey };
            }
    }
}