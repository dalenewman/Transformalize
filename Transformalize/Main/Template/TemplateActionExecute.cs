using System.IO;

namespace Transformalize.Main {
    public class TemplateActionExecute : TemplateActionHandler {

        public override void Handle(TemplateAction action) {
            TflLogger.Info(string.Empty, string.Empty, "Running {0}.", action.File);

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

                executable.OutputDataReceived += (sender, args) => TflLogger.Info(string.Empty, string.Empty, args.Data);
                executable.Start();
                executable.BeginOutputReadLine();
                executable.WaitForExit();
            } else {
                TflLogger.Warn(action.ProcessName, string.Empty, "Couldn't find and execute {0}.", action.File);
            }
        }
    }
}