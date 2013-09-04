using System.IO;
using Transformalize.Core.Process_;

namespace Transformalize.Core.Template_
{
    public class TemplateActionRun : TemplateActionHandler
    {
        private readonly string _fileName;

        public TemplateActionRun(string fileName)
        {
            _fileName = fileName;
        }

        public override void Handle(TemplateAction action)
        {
            var fileInfo = new FileInfo(_fileName);
            if (fileInfo.Exists)
            {
                var script = File.ReadAllText(fileInfo.FullName);
                if (!string.IsNullOrEmpty(script))
                {
                    var response = action.Connection.ScriptRunner.Execute(script);
                    if (response.Success)
                    {
                        Log.Info("{0} | {1} ran. It affected {2} rows.", Process.Name, action.TemplateName, response.RowsAffected < 0 ? 0 : response.RowsAffected);
                    }
                    else
                    {
                        Log.Warn("{0} | {1} failed", Process.Name, action.TemplateName);
                        foreach (var message in response.Messages)
                        {
                            Log.Warn(message);
                        }
                    }
                }
                else
                {
                    Log.Warn("{0} | {1} is empty.", Process.Name, action.TemplateName);
                }
            }
            else
            {
                Log.Warn("{0} | rendered output from {1} is not available.  It will not run.", Process.Name, action.TemplateName);
            }
            
        }
    }
}