using System;
using Transformalize.Libs.Cfg.Net.fastJSON;
using Transformalize.Libs.EnterpriseLibrary.Validation;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using ValidationResult = Transformalize.Libs.EnterpriseLibrary.Validation.ValidationResult;

namespace Transformalize.Operations.Validate {
    public class JsonValidator : ValueValidator<string> {
        public JsonValidator(string tag, bool negated)
            : base(string.Empty, tag, negated) { }

        protected override void DoValidate(string objectToValidate, object currentTarget, string key, ValidationResults validationResults) {
            try {
                JSON.Parse(objectToValidate);
                if (Negated) {
                    validationResults.AddResult(new ValidationResult("The object is JSON", objectToValidate, key, Tag, this));
                }
            } catch (Exception ex) {
                var message = string.Format(MessageTemplate, key, objectToValidate, ex.Message);
                validationResults.AddResult(new ValidationResult(message, objectToValidate, key, Tag, this));
            }
        }

        protected override string DefaultNonNegatedMessageTemplate {
            get { return string.Empty; }
        }

        protected override string DefaultNegatedMessageTemplate {
            get { return string.Empty; }
        }
    }
}