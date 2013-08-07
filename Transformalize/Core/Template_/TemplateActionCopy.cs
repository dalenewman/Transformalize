using System.IO;

namespace Transformalize.Core.Template_
{
    public class TemplateActionCopy : TemplateActionHandler
    {
        private readonly string _fileToCopy;

        public TemplateActionCopy(string fileToCopy)
        {
            _fileToCopy = fileToCopy;
        }

        public override void Handle(TemplateAction action)
        {
            var actionFile = string.IsNullOrEmpty(action.File)
                                 ? string.Empty
                                 : new FileInfo(action.File).FullName;

            if (actionFile != string.Empty)
            {
                File.Copy(_fileToCopy, actionFile, true);
                Log.Info("{0} | Copied {1} template output to {2}.", action.ProcessName, action.TemplateName, actionFile);
            }
            else
            {
                Log.Warn("{0} | Can't copy {1} template output without file attribute set.", action.ProcessName, action.TemplateName);
            }
        }
    }
}