using System.IO;

namespace Transformalize.Main {

    public class TemplateActionExecute : TemplateActionHandler {
        private readonly Process _process;

        public TemplateActionExecute(Process process) {
            _process = process;
        }

        public override void Handle(TemplateAction action) {
            _process.Logger.Info("Running {0}.", action.File);

            var fileInfo = new FileInfo(action.File);

            if (fileInfo.Exists) {
                var executable = new System.Diagnostics.Process {
                    StartInfo = {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        FileName = fileInfo.FullName,
                        Arguments = action.Arguments,
                        CreateNoWindow = true
                    }
                };

                executable.OutputDataReceived += (sender, args) => _process.Logger.Info(args.Data);
                executable.Start();
                executable.BeginOutputReadLine();
                executable.WaitForExit();
            } else {
                _process.Logger.Warn("Couldn't find and execute {0}.", action.File);
            }
        }
    }
}