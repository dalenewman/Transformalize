using System;
using System.IO;
using Transformalize.Core.Process_;
using Transformalize.Libs.Rhino.Etl.Core;

namespace Transformalize.Core.Template_
{
    public class TemplateManager : WithLoggingMixin
    {
        private readonly Process _process;
        private readonly char[] _trim = new[] { '\\' };
        public TemplateManager(Process process)
        {
            _process = process;
        }

        public void Manage()
        {

            var folder = GetTemporaryFolder();
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            foreach (var pair in _process.Templates)
            {
                var result = pair.Value.Render();
                Info("{0} | Rendered {1} template.", _process.Name, pair.Value.Name);

                var renderedInfo = new FileInfo(folder.TrimEnd(_trim) + @"\" + pair.Value.Name + ".temp.txt");
                File.WriteAllText(renderedInfo.FullName, result);

                if (!_process.Options.PerformTemplateActions)
                    continue;

                foreach (var action in pair.Value.Actions)
                {

                    switch (action.Action.ToLower())
                    {
                        case "copy":
                            new TemplateActionCopy(renderedInfo.FullName).Handle(action);
                            break;

                        case "open":
                            new TemplateActionOpen(renderedInfo.FullName).Handle(action);
                            break;

                        case "web":
                            new TemplateActionWeb().Handle(action);
                            break;

                        default:
                            Warn("{0} | The {1} action is not implemented.", _process.Name, action.Action);
                            break;

                    }
                }
            }
        }

        public string GetTemporaryFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).TrimEnd(_trim) + @"\Tfl\" + _process.Name;
        }
    }
}