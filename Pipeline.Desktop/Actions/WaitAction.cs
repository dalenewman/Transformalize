using System.Threading;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Desktop.Actions {
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