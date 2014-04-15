using System;
using System.Collections.Generic;
using System.Threading;
using Transformalize.Extensions;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class FilterOperation : ShouldRunOperation {
        private readonly object _value;
        private readonly ComparisonOperator _comparisonOperator;
        private int _filteredOutCount;

        public FilterOperation(string inKey, string outKey, string outType, object value, ComparisonOperator comparisonOperator)
            : base(inKey, outKey) {
            _value = Common.GetObjectConversionMap()[outType](value);
            _comparisonOperator = comparisonOperator;

            base.OnFinishedProcessing += FilterOperation_OnFinishedProcessing;
            Name = string.Format("FilterOperation ({0})", outKey);
        }

        void FilterOperation_OnFinishedProcessing(Libs.Rhino.Etl.Operations.IOperation obj) {
            if (_filteredOutCount > 0) {
                Info("Filtered out {0} row{1}.", _filteredOutCount, _filteredOutCount.Plural());
            }
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    if (Common.CompareMap[_comparisonOperator](row[InKey], _value)) {
                        yield return row;
                    } else {
                        Interlocked.Increment(ref _filteredOutCount);
                    }
                } else {
                    Interlocked.Increment(ref SkipCount);
                    yield return row;
                }
            }
        }
    }
}