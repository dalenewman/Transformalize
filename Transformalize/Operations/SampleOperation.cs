using System;
using System.Collections.Generic;
using System.Threading;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Operations.Transform;

namespace Transformalize.Operations
{
    public class SampleOperation : ShouldRunOperation {
        private readonly decimal _sampleRate;

        public SampleOperation(decimal sample)
            : base(string.Empty, string.Empty) {
            _sampleRate = sample >= 1m ? sample * .01m : sample;
            Name = string.Format("SampleOperation ({0:##} PERCENT)", sample);
            IsFilter = true;
            }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (_sampleRate >= Convert.ToDecimal(Guid.NewGuid().GetHashCode() & 0x7fffffff) / Convert.ToInt32(0x7fffffff)) {
                    yield return row;
                } else {
                    Interlocked.Increment(ref SkipCount);
                }
            }
        }
    }
}