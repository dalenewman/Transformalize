using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Desktop.Actions {
    public class LogAction : IAction {

        private readonly IContext _context;
        private readonly Action _action;
        private readonly char _level;

        public LogAction(IContext context, Action action) {
            _context = context;
            _action = action;
            _level = action.Level[0];
        }

        public ActionResponse Execute() {
            switch (_level) {
                case 'e':
                    _context.Error(_action.Message);
                    break;
                case 'd':
                    _context.Debug((() => _action.Message));
                    break;
                case 'w':
                    _context.Warn(_action.Message);
                    break;
                default:
                    _context.Info(_action.Message);
                    break;
            }
            return new ActionResponse(200, _action.Message) { Action = _action };
        }
    }
}