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
                Info("Skipped {0} row{1}.", SkipCount, SkipCount.Plural());
            }
        }
    }
}