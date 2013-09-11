using System.IO;
using Transformalize.Core.Process_;

namespace Transformalize.Core.Template_
{
    public class TemplateActionRun : TemplateActionHandler
    {
        public override void Handle(TemplateAction action)
        {
            var fileInfo = new FileInfo(action.RenderedFile);
            if (fileInfo.Exists)
            {
                var script = File.ReadAllText(fileInfo.FullName);
                if (!string.IsNullOrEmpty(script))
                {
                    var response = action.Connection.ExecuteScript(script);
                    if (response.Success)
                    {
                        Log.Info("{0} ran successfully.", action.TemplateName);
                        Log.Debug("{0} affected {1} rows.", action.TemplateName, response.RowsAffected < 0 ? 0 : response.RowsAffected);
                    }
                    else
                    {
                        Log.Warn("{0} failed", action.TemplateName);
                        foreach (var message in response.Messages)
                        {
                            Log.Warn(message);
                        }
                    }
                }
                else
                {
                    Log.Warn("{0} is empty.", action.TemplateName);
                }
            }
            else
            {
                Log.Warn("rendered output from {0} is not available.  It will not run.", action.TemplateName);
            }
            
        }
    }
}