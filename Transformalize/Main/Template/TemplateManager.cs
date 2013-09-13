#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System.IO;
using Transformalize.Libs.NLog;

namespace Transformalize.Main.Template_
{
    public class TemplateManager
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly Process _process;
        private readonly char[] _trim = new[] {'\\'};

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

                if (!_process.Options.PerformTemplateActions)
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