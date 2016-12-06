using System.Threading;
using Transformalize.Actions;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Desktop.Actions {
    public class WaitAction : IAction {
        private readonly Action _action;
        public WaitAction(Action action) {
            _action = action;
        }

        public ActionResponse Execute() {
            var message = _action.Type == "wait" ? $"Waited for {_action.TimeOut} ms" : $"Slept for {_action.TimeOut} ms";
            Thread.Sleep(_action.TimeOut);
            return new ActionResponse(200, message) { Action = _action };
        }
    }
}