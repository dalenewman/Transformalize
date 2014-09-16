using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;
using Transformalize.Operations;

namespace Transformalize.Processes {
    public class MasterJoinProcess : EtlProcess {
        private readonly Process _process;
        private readonly CollectorOperation _collector;

        public MasterJoinProcess(Process process, ref CollectorOperation collector)
            : base(process) {
            _process = process;
            _collector = collector;
        }

        protected override void Initialize() {
            Register(new RowsOperation(_process.Relationships.First().LeftEntity.Rows));
            foreach (var rel in _process.Relationships) {
                Register(new EntityJoinOperation(_process, rel).Right(new RowsOperation(rel.RightEntity.Rows)));
            }
            Register(_collector);
        }
    }
}