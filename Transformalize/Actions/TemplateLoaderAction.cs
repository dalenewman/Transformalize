using System.Collections.Generic;
using System.Linq;
using Cfg.Net.Contracts;
using Cfg.Net.Loggers;
using Transformalize.Contracts;

namespace Transformalize.Actions {
    public class TemplateLoaderAction : IAction {
        private readonly IContext _context;
        private readonly IReader _reader;

        public TemplateLoaderAction(IContext context, IReader reader) {
            _context = context;
            _reader = reader;
        }

        public ActionResponse Execute() {
            var logger = new MemoryLogger();
            var code = 200;
            var message = "Ok";
            foreach (var template in _context.Process.Templates.Where(t => t.Enabled && !t.Actions.Any())) {
                if (template.File != string.Empty) {
                    template.Content = _reader.Read(template.File, new Dictionary<string, string>(), logger);
                    if (logger.Errors().Any()) {
                        code = 500;
                        foreach (var error in logger.Errors()) {
                            message += error + " ";
                        }
                        break;
                    }
                }
            }

            return new ActionResponse(code, message) {
                Action = new Configuration.Action {
                    Before = true,
                    After = false,
                    Type = "internal",
                    ErrorMode = "abort",
                    Description = "load template"
                }
            };
        }
    }
}