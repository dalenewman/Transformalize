using System;
using System.Linq;
using Transformalize.Model;
using Transformalize.Operations;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Processes {

    public class UpdateMasterProcess : EtlProcess {

        private readonly Process _process;
        private readonly int[] _tflId;

        public UpdateMasterProcess(Process process)
            : base(process.Name) {
            _process = process;
            _tflId = process.Entities.Select(e => e.Value.TflId).Distinct().ToArray();
        }

        protected override void Initialize() {
            foreach (var entity in _process.Entities) {
                Register(new EntityUpdateMaster(_process, entity.Value));
            }
        }

        protected override void PostProcessing() {

            var errors = GetAllErrors().ToArray();
            if (errors.Any()) {
                foreach (var error in errors) {
                    Error(error.InnerException, "Message: {0}\r\nStackTrace:{1}\r\n", error.Message, error.StackTrace);
                }
                throw new InvalidOperationException("Houstan.  We have a problem.");
            }

            base.PostProcessing();
        }

    }

}
