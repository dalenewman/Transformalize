using Transformalize.Libs.EnterpriseLibrary.Validation;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using Transformalize.Libs.fastJSON;
using ValidationResult = Transformalize.Libs.EnterpriseLibrary.Validation.ValidationResult;

namespace Transformalize.Operations.Validate {
    public class JsonValidator : ValueValidator<string> {
        public JsonValidator(string tag, bool negated)
            : base(string.Empty, tag, negated){}

        protected override void DoValidate(string objectToValidate, object currentTarget, string key, ValidationResults validationResults) {
            var result = JSON.Instance.Validate(objectToValidate);
            if (result.IsValid == !Negated)
                return;
            var message = string.Format(MessageTemplate, key, objectToValidate, result.Message);
            validationResults.AddResult(new ValidationResult(message, objectToValidate, key, Tag, this));
        }

        protected override string DefaultNonNegatedMessageTemplate {
            get { return string.Empty; }
        }

        protected override string DefaultNegatedMessageTemplate {
            get { return string.Empty; }
        }
    }
}