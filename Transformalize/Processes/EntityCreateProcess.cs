using System.Linq;
using Transformalize.Model;
using Transformalize.Operations;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Processes {

    public class EntityCreateProcess : EtlProcess {

        private readonly Process _process;

        public EntityCreateProcess(Process process)
            : base(process.Name) {
            _process = process;
        }

        protected override void Initialize() {
            foreach (var pair in _process.Entities) {
                Register(new EntityCreate(pair.Value, _process));
            }
        }

        protected override void PostProcessing() {

            var errors = GetAllErrors().ToArray();
            if (errors.Any()) {
                foreach (var error in errors) {
                    Error(error.InnerException, "Message: {0}\r\nStackTrace:{1}\r\n", error.Message, error.StackTrace);
                }
                throw new TransformalizeException("Entity Creation Error!");
            }

            base.PostProcessing();
        }

    }

}
