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

    public class TemplateActionCopy : TemplateActionHandler {

        public override void Handle(TemplateAction action) {

            var actionFile = string.IsNullOrEmpty(action.File)
                ? string.Empty
                : new FileInfo(action.File).FullName;

            if (actionFile != string.Empty) {
                var fileInfo = new FileInfo(actionFile);
                if (fileInfo.Directory == null || fileInfo.Directory != null && fileInfo.Directory.Exists) {
                    File.Copy(action.RenderedFile, actionFile, true);
                    Log.Info("Copied {0} template output to {1}.", action.TemplateName, actionFile);
                } else {
                    Log.Warn("Unable to copy file to folder {0}.  The folder doesn't exist.", fileInfo.DirectoryName);
                }
            } else {
                Log.Warn("Can't copy {0} template output without file attribute set.", action.TemplateName);
            }
        }
    }
}