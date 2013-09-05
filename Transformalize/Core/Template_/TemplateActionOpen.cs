using System.IO;

namespace Transformalize.Core.Template_
{
    public class TemplateActionOpen : TemplateActionHandler
    {

        public override void Handle(TemplateAction action)
        {
            var actionFile = string.IsNullOrEmpty(action.File)
                                 ? string.Empty
                                 : new FileInfo(action.File).FullName;

            var openFile = actionFile == string.Empty ? action.RenderedFile : actionFile;
            System.Diagnostics.Process.Start(openFile);
            Log.Info("Opened file {0}.", openFile);
        }
    }
}