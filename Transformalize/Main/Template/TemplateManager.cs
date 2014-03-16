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

using System;
using System.IO;
using Transformalize.Libs.NLog;

namespace Transformalize.Main {

    public class TemplateManager {

        private readonly Logger _log = LogManager.GetLogger(string.Empty);
        private readonly Process _process;
        private readonly char[] _trim = new[] { '\\' };

        public TemplateManager(Process process) {
            _process = process;
        }

        public void Manage() {
            var folder = Common.GetTemporaryFolder(_process.Name);

            foreach (var pair in _process.Templates) {

                string result;
                var template = pair.Value;

                if (template.IsUsedInPipeline)
                    continue;

                try {
                    result = template.Render();
                } catch (Exception e) {
                    result = e.Message;
                    _log.Warn("Template {0} failed to render. {1}. If the template is depending on pipe-line variables, make sure it is referenced in a template transform.", template.Name, e.Message);
                    _log.Debug(e.StackTrace);
                }

                var renderedInfo = new FileInfo(folder.TrimEnd(_trim) + @"\" + template.Name + new FileInfo(template.File).Extension.ToLower().Replace("cshtml", "txt"));
                File.WriteAllText(renderedInfo.FullName, result);

                if (!_process.Options.PerformTemplateActions)
                    continue;

                foreach (var action in template.Actions) {
                    action.RenderedFile = renderedInfo.FullName;

                    switch (action.Action.ToLower()) {
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

                        case "execute":
                            new TemplateActionExecute().Handle(action);
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