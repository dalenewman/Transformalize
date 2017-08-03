using RethinkDb;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Provider.RethinkDB {
    public class RethinkDbOutputController : BaseOutputController {

        IConnectionFactory _factory;
        public RethinkDbOutputController(
            OutputContext context, 
            IAction initializer, 
            IInputProvider inputProvider, 
            IOutputProvider outputProvider,
            IConnectionFactory factory
            ) : base(context, initializer, inputProvider, outputProvider) {
            _factory = factory;
        }

        public override void Start() {
            base.Start();
        }
    }
}
