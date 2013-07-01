using System;
using System.Linq;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize {

    public class TransformProcess : EtlProcess {

        private readonly Process _process;

        public TransformProcess(Process process) : base(process.Name) {
            _process = process;
        }

        protected override void Initialize() {
            // TODO: make a transform process that does this...
            // get all the fields needed to satisfy transform parameters
            // query fields from output table where TflId in (@TflIds), also return TflKey
            // run the transforms on them.
            // run batch update process on transformed output, using TflKey as join
            //var batchIds = process.Entities.Select(e => e.Value.TflId).ToArray();
            
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
