using System.IO;
using Transformalize.Core.Process_;

namespace Transformalize.Core.Template_
{
    public class TemplateActionCopy : TemplateActionHandler
    {
        public override void Handle(TemplateAction action)
        {
            var actionFile = string.IsNullOrEmpty(action.File)
                                 ? string.Empty
                                 : new FileInfo(action.File).FullName;

            if (actionFile != string.Empty)
            {
                File.Copy(action.RenderedFile, actionFile, true);
                Log.Info("Copied {0} template output to {1}.", action.TemplateName, actionFile);
            }
            else
            {
                Log.Warn("Can't copy {0} template output without file attribute set.", action.TemplateName);
            }
        }
    }
}