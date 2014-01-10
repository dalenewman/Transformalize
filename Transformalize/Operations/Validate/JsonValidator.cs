using System.Linq;
using Transformalize.Libs.EnterpriseLibrary.Validation;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using Transformalize.Libs.fastJSON;

namespace Transformalize.Operations.Validate {
    public class JsonValidator : ValueValidator<string> {
        public JsonValidator(string messageTemplate, string tag) : base(messageTemplate, tag, false) { }

        protected override void DoValidate(string objectToValidate, object currentTarget, string key, ValidationResults validationResults) {
            var result = JSON.Instance.Validate(objectToValidate);
            if (result.IsValid)
                return;
            var message = string.Format(MessageTemplate, key, Tag, result.Message);
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