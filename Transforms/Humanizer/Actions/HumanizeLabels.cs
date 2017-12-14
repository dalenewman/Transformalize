using Humanizer;
using Transformalize.Actions;
using Transformalize.Contracts;
using Action = Transformalize.Configuration.Action;

namespace Transformalize.Transforms.Humanizer.Actions {
    public class HumanizeLabels : IAction {

        private readonly IContext _context;
        private readonly Action _action;

        public HumanizeLabels(IContext context, Action action) {
            _context = context;
            _action = action;
        }
        public ActionResponse Execute() {
            foreach (var parameter in _context.Process.GetActiveParameters()) {
                if (parameter.Label == string.Empty || parameter.Label == parameter.Name) {
                    parameter.Label = parameter.Label.Humanize(LetterCasing.Title);
                }
            }
            foreach (var entity in _context.Process.Entities) {
                if (entity.Label == string.Empty || entity.Label == entity.Alias) {
                    entity.Label = entity.Alias.Humanize(LetterCasing.Title);
                }
                foreach (var field in entity.GetAllFields()) {
                    if (field.Label == string.Empty || field.Label == field.Alias) {
                        field.Label = field.Alias.Humanize(LetterCasing.Title);
                    }
                }
            }

            return new ActionResponse(200) { Action = _action };
        }
    }
}
