using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.EnterpriseLibrary.Validation;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations.Validate {

    public class ValidationOperation : AbstractOperation {

        private readonly string _keyToValidate;
        private readonly bool _messageAppend;
        private readonly bool _ignoreEmpty;
        private readonly bool _messageOutput;
        private readonly bool _isTypeConversion;

        public Validator Validator { get; set; }
        public bool ValidateRow { get; set; }
        public string ResultKey { get; set; }
        public string MessageKey { get; set; }

        public ValidationOperation(string keyToValidate, string resultKey, string messageKey, bool messageAppend, bool ignoreEmpty = false) {
            ResultKey = resultKey;
            MessageKey = messageKey;

            _keyToValidate = keyToValidate;
            _messageAppend = messageAppend;
            _ignoreEmpty = ignoreEmpty;
            _messageOutput = !messageKey.Equals(string.Empty);
            _isTypeConversion = this is TypeConversionValidatorOperation;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            foreach (var row in rows) {
                if (_isTypeConversion) {
                    var stringValue = row[_keyToValidate].ToString();
                    if (_ignoreEmpty && stringValue.Equals(string.Empty)) {
                        row[ResultKey] = true;
                    } else {
                        Validate(stringValue, row);
                    }
                } else {
                    Validate(row[_keyToValidate], row);
                }
                
                yield return row;
            }
        }

        private void Validate(object value, Row row) {
            var results = new ValidationResults();
            Validator.DoValidate(value, row, _keyToValidate, results);
            row[ResultKey] = results.IsValid;

            if (!_messageOutput || results.IsValid)
                return;

            var message = results.First().Message;
            row[MessageKey] = _messageAppend ?
                string.Concat(row[MessageKey] ?? string.Empty, " ", message).Trim(' ') :
                message;
        }

    }
}