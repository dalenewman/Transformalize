using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Impl;

namespace Transformalize.Providers.File {
    public class FileOutputController : BaseOutputController {
        public FileOutputController(OutputContext context, IAction initializer, IInputProvider inputProvider, IOutputProvider outputProvider) : base(context, initializer, inputProvider, outputProvider) {
            
        }
    }
}
