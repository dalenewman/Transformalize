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

            var from = string.Empty;
            var to = string.Empty;
            var rendered = false;

            if (!action.From.Equals(string.Empty) && !action.To.Equals(string.Empty)) {
                from = action.From;
                to = action.To;
            } else if (!action.TemplateName.Equals(string.Empty) && !action.RenderedFile.Equals(string.Empty)) {
                from = action.RenderedFile;
                to = action.File.Equals(string.Empty) ? action.To : action.File;
                rendered = true;
            }

            var fromInfo = new FileInfo(from);
            if (!fromInfo.Exists) {
                Log.Warn("Can't copy {0} to {1}, because {0} doesn't exist.", rendered ? action.TemplateName + " rendered output" : from, to);
                return;
            }

            var toInfo = new FileInfo(to);

            if (toInfo.Directory != null && toInfo.Directory.Exists) {
                File.Copy(fromInfo.FullName, toInfo.FullName, true);
                Log.Info("Copied {0} to {1}.", rendered ? action.TemplateName + " rendered output" : from, to);
            } else {
                Log.Warn("Unable to copy file to folder {0}.  The folder doesn't exist.", toInfo.DirectoryName);
            }
        }
    }
}