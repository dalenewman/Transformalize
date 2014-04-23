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
        private readonly char[] _trim = { '\\' };

        public TemplateManager(Process process) {
            _process = process;
        }

        public void Manage() {

            if (!_process.Options.RenderTemplates)
                return;

            var folder = Common.GetTemporaryFolder(_process.Name);

            foreach (var pair in _process.Templates) {

                var template = pair.Value;

                if (template.IsUsedInPipeline) {
                    continue;
                }

                if (!template.Enabled) {
                    _log.Warn("Template {0} is disabled.", template.Name);
                    continue;
                }

                if (!_process.UpdatedAnything() && template.Conditional)
                {
                    continue;
                }

                var fullName = template.Contents.FileName;

                if (template.ShouldRender) {
                    try {
                        fullName = new FileInfo(folder.TrimEnd(_trim) + @"\" + template.Name + new FileInfo(template.Contents.FileName).Extension.ToLower().Replace("cshtml", "html")).FullName;
                        File.WriteAllText(fullName, template.Render());
                    } catch (Exception e) {
                        _log.Warn("Template {0} failed to render. {1}. If the template is depending on pipe-line variables, make sure it is referenced in a template transform.", template.Name, e.Message);
                        _log.Debug(e.StackTrace);
                    }
                }

                if (!_process.Options.PerformTemplateActions)
                    continue;

                foreach (var action in template.Actions) {
                    action.Handle(fullName);
                }
            }
        }
    }
}