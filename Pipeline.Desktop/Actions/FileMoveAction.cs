using System;
using System.IO;
using Pipeline.Contracts;
using Action = Pipeline.Configuration.Action;

namespace Pipeline.Desktop.Actions {
    public class FileMoveAction : IAction {
        private readonly IContext _context;
        private readonly Action _action;

        public FileMoveAction(IContext context, Action action) {
            _context = context;
            _action = action;
        }

        public ActionResponse Execute() {
            var from = new FileInfo(_action.From);
            var to = new FileInfo(_action.To);

            if (!Path.HasExtension(to.FullName)) {
                to = new FileInfo(Path.Combine(to.FullName, from.Name));
            }

            _context.Info("Moving {0} to {1}", from.Name, to.Name);
            try {
                File.Move(from.FullName, to.FullName);
                return new ActionResponse(200, "Ok");
            } catch (Exception ex) {
                return new ActionResponse(500, ex.Message);
            }
        }
    }
}