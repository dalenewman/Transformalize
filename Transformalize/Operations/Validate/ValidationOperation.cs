using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.EnterpriseLibrary.Validation;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations.Validate
{
    public class ValidationOperation : AbstractOperation {
        private readonly string _inKey;
        private readonly string _outKey;
        private readonly bool _append;
        public Validator Validator { get; set; }

        public ValidationOperation(string inKey, string outKey, bool append) {
            _inKey = inKey;
            _outKey = outKey;
            _append = append;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                var results = Validator.Validate(row[_inKey]);
                if (!results.IsValid) {
                    row[_outKey] = _append ? (row[_outKey] + " " + results.First().Message).Trim(' ') : results.First().Message;
                }
                yield return row;
            }
        }
    }
}