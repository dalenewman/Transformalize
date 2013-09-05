using System.IO;
using Transformalize.Core.Process_;
using Transformalize.Libs.NLog;

namespace Transformalize.Core.Template_
{
    public class TemplateManager
    {
        private readonly Process _process;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly char[] _trim = new[] { '\\' };

        public TemplateManager(Process process)
        {
            _process = process;
        }

        public void Manage()
        {

            var folder = Common.GetTemporaryFolder(_process.Name);

            foreach (var pair in _process.Templates)
            {
                var result = pair.Value.Render();
                _log.Debug("Rendered {0} template.", pair.Value.Name);

                var renderedInfo = new FileInfo(folder.TrimEnd(_trim) + @"\" + pair.Value.Name + ".temp.txt");
                File.WriteAllText(renderedInfo.FullName, result);

                if (!Process.Options.PerformTemplateActions)
                    continue;

                foreach (var action in pair.Value.Actions)
                {
                    action.RenderedFile = renderedInfo.FullName;
                    switch (action.Action.ToLower())
                    {
                        case "copy":
                            new TemplateActionCopy().Handle(action);
                            break;

                        case "open":
                            new TemplateActionOpen().Handle(action);
                            break;

                        case "run":
                            new TemplateActionRun().Handle(action);
                            break;

                        case "web":
                            new TemplateActionWeb().Handle(action);
                            break;

                        default:
                            _log.Warn("The {0} action is not implemented.", action.Action);
                            break;

                    }
                }
            }
        }

    }
}