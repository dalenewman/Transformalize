using System.Linq;
using Transformalize.Data;
using Transformalize.Model;
using Transformalize.Operations;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Processes {

    public class InitializationProcess : EtlProcess {

        private readonly Process _process;
        private readonly ITflWriter _tflWriter;
        private readonly IViewWriter _viewWriter;

        public InitializationProcess(Process process, ITflWriter tflWriter = null, IViewWriter viewWriter = null) : base(process.Name) {
            _process = process;
            _tflWriter = tflWriter ?? new SqlServerTflWriter(ref process);
            _viewWriter = viewWriter ?? new SqlServerViewWriter(ref process);

            _tflWriter.Initialize();
            _viewWriter.Drop();
        }

        protected override void Initialize() {
            foreach (var pair in _process.Entities) {
                Register(new EntityDrop(pair.Value));
                Register(new EntityCreate(pair.Value, _process));
            }
        }

        protected override void PostProcessing() {
            _viewWriter.Create();

            var errors = GetAllErrors().ToArray();
            if (errors.Any()) {
                foreach (var error in errors) {
                    Error(error.InnerException, "Message: {0}\r\nStackTrace:{1}\r\n", error.Message, error.StackTrace);
                }
                throw new TransformalizeException("Initialization Error!");
            }

            base.PostProcessing();
        }

    }

}
