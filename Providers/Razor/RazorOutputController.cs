using Transformalize.Context;
using Transformalize.Contracts;

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
