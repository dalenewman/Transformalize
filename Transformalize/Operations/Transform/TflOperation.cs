using System;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations.Transform {
    public abstract class TflOperation : AbstractOperation {
        public Func<Row, bool> ShouldRun = row => true;
        protected string InKey;
        protected string OutKey;

        protected TflOperation(string inKey, string outKey) {
            InKey = inKey;
            OutKey = outKey;
        }
    }
}