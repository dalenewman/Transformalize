using System;
using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {

    public class TimeSpanOperation : ShouldRunOperation {
        private readonly IParameters _parameters;

        public TimeSpanOperation(IParameters parameters, string outKey)
            : base(string.Empty, outKey) {
            _parameters = parameters;
            Name = string.Format("TimeSpanOperation ({0})", outKey);

            if (_parameters.Count < 2) {
                throw new TransformalizeException(EntityName, Logger, "Timespan requires 2 date parameters.");
            }
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var array = new[] { row[_parameters[0].Name] ?? _parameters[0].Value, row[_parameters[1].Name] ?? _parameters[1].Value };
                    var d1 = (DateTime)array[0];
                    var d2 = (DateTime)array[1];
                    row[OutKey] = d1 - d2;
                } else {
                    Skip();
                }
                yield return row;
            }
        }
    }

}