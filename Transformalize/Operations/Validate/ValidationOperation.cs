using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.EnterpriseLibrary.Validation;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations.Validate {

    public class ValidationOperation : AbstractOperation {

        private readonly string _keyToValidate;
        private readonly bool _messageAppend;
        private readonly bool _messageOutput;

        public Validator Validator { get; set; }
        public bool ValidateRow { get; set; }
        public string ResultKey { get; set; }
        public string MessageKey { get; set; }

        public ValidationOperation(string keyToValidate, string resultKey, string messageKey, bool messageAppend) {
            ResultKey = resultKey;
            MessageKey = messageKey;

            _keyToValidate = keyToValidate;
            _messageAppend = messageAppend;
            _messageOutput = !messageKey.Equals(string.Empty);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            foreach (var row in rows) {
                var results = new ValidationResults();
                var value = this is TypeConversionValidatorOperation ? row[_keyToValidate].ToString() : row[_keyToValidate];
                Validator.DoValidate(value, row, _keyToValidate, results);

                var valid = results.IsValid;
                row[ResultKey] = valid;
                if (_messageOutput && !valid) {
                    var message = results.First().Message;
                    row[MessageKey] = _messageAppend ?
                        string.Concat(row[MessageKey] ?? string.Empty, " ", message).Trim(' ') :
                        message;
                }
                yield return row;
            }
        }
    }
}