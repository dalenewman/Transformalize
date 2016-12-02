using System;
using Pipeline.Contracts;
using Pipeline.Extensions;
using Action = Pipeline.Configuration.Action;

namespace Pipeline.Desktop.Actions {
    public class PrintAction : IAction {
        private readonly Action _action;
        private readonly string _level;

        public PrintAction(Action action) {
            _action = action;
            _level = action.Level.Left(1);
        }

        public ActionResponse Execute() {

            if (_level == "e") {
                Console.Error.WriteLine(_action.Message);
            } else {
                Console.WriteLine(_action.Message);
            }
            return new ActionResponse(200, _action.Message) { Action = _action };
        }
    }
}