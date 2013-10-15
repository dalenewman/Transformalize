using System;
using System.Collections.Generic;
using Transformalize.Libs.EnterpriseLibrary.Validation;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using System.Linq;

namespace Transformalize.Operations.Validate {

    public class DateTimeRangeOperation : AbstractOperation {

        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly string _inKey;
        private readonly string _outKey;
        private readonly Validator _validator;
        private readonly RangeBoundaryType _lowerBoundary;
        private readonly RangeBoundaryType _upperBoundary;

        public DateTimeRangeOperation(string inKey, string outKey, DateTime lowerBound, string lowerBoundary, DateTime upperBound, string upperBoundary, string messageTemplate, bool negated) {
            _inKey = inKey;
            _outKey = outKey;
            _lowerBoundary = lowerBoundary.Equals("inclusive", IC) ? RangeBoundaryType.Inclusive : RangeBoundaryType.Exclusive;
            _upperBoundary = upperBoundary.Equals("inclusive", IC) ? RangeBoundaryType.Inclusive : RangeBoundaryType.Exclusive;
            _validator = new DateTimeRangeValidator(lowerBound, _lowerBoundary, upperBound, _upperBoundary, messageTemplate, negated) { Tag = inKey };
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                var results = _validator.Validate(row[_inKey]);
                if (!results.IsValid) {
                    row[_outKey] = (row[_outKey] + " " + results.First().Message).Trim(' ');
                }
                yield return row;
            }
        }
    }


    public class ContainsCharactersOperation : AbstractOperation {

        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly string _inKey;
        private readonly string _outKey;
        private readonly Validator _validator;
        private readonly ContainsCharacters _scope;

        public ContainsCharactersOperation(string inKey, string outKey, string characters, string scope, string messageTemplate, bool negated) {
            _inKey = inKey;
            _outKey = outKey;
            _scope = scope.Equals("all", IC) ? ContainsCharacters.All : ContainsCharacters.Any;
            _validator = new ContainsCharactersValidator(characters, _scope, messageTemplate, negated) { Tag = inKey };
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                var results = _validator.Validate(row[_inKey]);
                if (!results.IsValid) {
                    row[_outKey] =  (row[_outKey] + " " + results.First().Message).Trim(' ');
                }
                yield return row;
            }
        }
    }
}
