using System;
using System.IO;
using Transformalize.Core.Process_;
using Transformalize.Libs.NLog;

namespace Transformalize.Core.Template_
{
    public class TemplateManager
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly Process _process;
        private readonly char[] _trim = new[] { '\\' };
        public TemplateManager(Process process)
        {
            _process = process;
        }

        public void Manage()
        {

            var folder = Common.GetTemporaryFolder();

            foreach (var pair in Process.Templates)
            {
                var result = pair.Value.Render();
                _log.Info("{0} | Rendered {1} template.", Process.Name, pair.Value.Name);

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
                            _log.Warn("{0} | The {1} action is not implemented.", Process.Name, action.Action);
                            break;

                    }
                }
            }
        }

    }
}