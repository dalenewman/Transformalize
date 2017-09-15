using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Impl;

namespace Transformalize.Providers.Razor {
    public class RazorOutputController : BaseOutputController {
        public RazorOutputController(
            OutputContext context, 
            IAction initializer, 
            IInputProvider inputProvider, 
            IOutputProvider outputProvider
        ) : base(context, initializer, inputProvider, outputProvider){}

    }
}
