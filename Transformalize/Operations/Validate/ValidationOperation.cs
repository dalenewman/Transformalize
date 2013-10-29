using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.EnterpriseLibrary.Validation;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations.Validate {

    public class ValidationOperation : AbstractOperation {

        private readonly string _keyToValidate;
        private readonly string _outKey;
        private readonly bool _append;
        public Validator Validator { get; set; }
        public bool ValidateRow { get; set; }

        public ValidationOperation(string keyToValidate, string outKey, bool append) {
            _keyToValidate = keyToValidate;
            _outKey = outKey;
            _append = append;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                var results = new ValidationResults();
                Validator.DoValidate(row[_keyToValidate], row, _keyToValidate, results);
                if (!results.IsValid) {
                    row[_outKey] = _append ? (row[_outKey] + " " + results.First().Message).Trim(' ') : results.First().Message;
                }
                yield return row;
            }
        }
    }
}