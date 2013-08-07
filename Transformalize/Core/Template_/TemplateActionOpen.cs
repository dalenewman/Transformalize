using System.IO;

namespace Transformalize.Core.Template_
{
    public class TemplateActionOpen : TemplateActionHandler
    {
        private readonly string _alternateFile;

        public TemplateActionOpen(string alternateFile)
        {
            _alternateFile = alternateFile;
        }

        public override void Handle(TemplateAction action)
        {
            var actionFile = string.IsNullOrEmpty(action.File)
                                 ? string.Empty
                                 : new FileInfo(action.File).FullName;

            var openFile = actionFile == string.Empty ? _alternateFile : actionFile;
            System.Diagnostics.Process.Start(openFile);
            Log.Info("{0} | Opened file {1}.", action.ProcessName, openFile);
        }
    }
}