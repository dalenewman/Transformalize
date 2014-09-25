namespace Transformalize.Main {
    public class TemplateActionTfl : TemplateActionHandler {
        public override void Handle(TemplateAction action) {
            var processes = ProcessFactory.Create(action.Url);
            foreach (var process in processes) {
                process.ExecuteScaler();
            }
        }
    }
}