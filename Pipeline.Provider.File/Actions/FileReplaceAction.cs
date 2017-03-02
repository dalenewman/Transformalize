using System.IO;
using Transformalize.Actions;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Provider.File.Actions {
    public class FileReplaceAction : IAction {
        private readonly IContext _context;
        private readonly Action _action;
        private readonly FileInfo _fileInfo;

        public FileReplaceAction(IContext context, Action action) {
            _context = context;
            _action = action;
            _fileInfo = new FileInfo(_action.File);
        }

        public ActionResponse Execute() {
            var response = new ActionResponse { Action = _action };
            if (_fileInfo.Exists) {
                var content = System.IO.File.ReadAllText(_fileInfo.FullName);
                System.IO.File.WriteAllText(_fileInfo.FullName, content.Replace(_action.OldValue, _action.NewValue));
                _context.Info($"Replaced {_action.OldValue} with {_action.NewValue} in {_action.File}");
            } else {
                response.Code = 401;
                response.Message = $"File {_action.File} not found";
            }
            return response;
        }
    }
}