using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations
{
    public class MasterEntityIndex : AbstractOperation {
        private readonly Process _process;

        public MasterEntityIndex(Process process) {
            _process = process;
            Name = "MasterEntityIndex";
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            new MasterEntityIndexBuilder(_process).Create();
            return rows;
        }

    }
}