using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations
{
    public class SampleOperation : AbstractOperation {
        private readonly decimal _sampleRate;

        public SampleOperation(decimal sample) {
            _sampleRate = sample >= 1m ? sample * .01m : sample;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            var enumeratedRows = rows.ToArray();

            var total = enumeratedRows.Count();
            var take = Convert.ToInt32(Math.Round(total * _sampleRate, 0));
            var rnd = new Random();

            Info("Sampling {0} of {1} records ({2:P1}).", take, total, _sampleRate);

            return enumeratedRows.OrderBy(x => rnd.Next()).Take(take);
        }
    }
}