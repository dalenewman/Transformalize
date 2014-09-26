namespace Transformalize.Main {
    public class TemplateActionTfl : TemplateActionHandler {
        public override void Handle(TemplateAction action) {
            var processes = ProcessFactory.Create(action.Url);
            foreach (var process in processes) {
                TflLogger.Info(action.ProcessName, string.Empty, "Executing {0}", process.Name);
                process.ExecuteScaler();
            }
        }
    }
}