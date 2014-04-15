using System;
using Transformalize.Extensions;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations.Transform {
    public abstract class ShouldRunOperation : AbstractOperation {
        public Func<Row, bool> ShouldRun = row => true;
        protected string InKey;
        protected string OutKey;
        protected int SkipCount = 0;

        protected ShouldRunOperation(string inKey, string outKey) {
            base.OnFinishedProcessing += TflOperation_OnFinishedProcessing;
            InKey = inKey;
            OutKey = outKey;
        }

        void TflOperation_OnFinishedProcessing(IOperation obj) {
            if (SkipCount > 0) {
                Info("Skipped {0} of {1} row{2}.", SkipCount, obj.Statistics.OutputtedRows, obj.Statistics.OutputtedRows.Plural());
            }
            var seconds = Convert.ToInt64(obj.Statistics.Duration.TotalSeconds);
            Info("Completed {0} rows in {1}: {2} second{3}.", obj.Statistics.OutputtedRows, Name, seconds, seconds.Plural());
        }
    }
}