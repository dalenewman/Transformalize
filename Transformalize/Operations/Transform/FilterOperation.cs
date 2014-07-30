using System.Collections.Generic;
using System.Threading;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class FilterOperation : ShouldRunOperation {
        private readonly object _value;
        private readonly ComparisonOperator _comparisonOperator;

        public FilterOperation(string inKey, string outKey, string outType, object value, ComparisonOperator comparisonOperator)
            : base(inKey, outKey) {
            _value = Common.GetObjectConversionMap()[outType](value);
            _comparisonOperator = comparisonOperator;

            Name = string.Format("FilterOperation ({0})", outKey);
            IsFilter = true;
            }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    if (Common.CompareMap[_comparisonOperator](row[InKey], _value)) {
                        yield return row;
                    } else {
                        Interlocked.Increment(ref SkipCount);
                    }
                } else {
                    yield return row;
                }
            }
        }
    }
}