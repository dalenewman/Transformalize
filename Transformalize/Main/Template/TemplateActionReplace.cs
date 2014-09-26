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

namespace Transformalize.Main {

    public class TemplateActionReplace : TemplateActionHandler {
        private readonly string _oldValue;
        private readonly string _newValue;

        public TemplateActionReplace()
            : this(string.Empty, string.Empty) {
        }

        public TemplateActionReplace(string oldValue, string newValue) {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override void Handle(TemplateAction action) {

            if (!_oldValue.Equals(string.Empty))
                action.OldValue = _oldValue;

            if (!_newValue.Equals(string.Empty))
                action.NewValue = _newValue;

            var fileName = string.Empty;
            if (!string.IsNullOrEmpty(action.File)) {
                fileName = action.File;
            } else {
                if (!string.IsNullOrEmpty(action.RenderedFile)) {
                    fileName = action.RenderedFile;
                }
            }

            var fileInfo = new FileInfo(fileName);

            if (fileInfo.Exists) {
                var content = File.ReadAllText(fileInfo.FullName);
                File.WriteAllText(fileInfo.FullName, content.Replace(action.OldValue, action.NewValue));
                TflLogger.Info(action.ProcessName, string.Empty, "Performed {0} action on {1}.", action.Action, fileInfo.Name);
            } else {
                if (action.TemplateName.Equals(string.Empty)) {
                    TflLogger.Warn(action.ProcessName, string.Empty, "Skipping {0} action. File '{1}' does not exist.", action.Action, fileName);
                } else {
                    TflLogger.Warn(action.ProcessName, string.Empty, "Skipping {0} action in {1} template. Niether file '{2}' nor rendered file '{3}' exist.", action.Action, action.TemplateName, action.File, action.RenderedFile);
                }
            }
        }
    }
}