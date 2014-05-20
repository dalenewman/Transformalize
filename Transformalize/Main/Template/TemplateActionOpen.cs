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
using Transformalize.Main.Providers;

namespace Transformalize.Main {

    public class TemplateActionOpen : TemplateActionHandler {

        public override void Handle(TemplateAction action) {

            var actionFile = string.IsNullOrEmpty(action.File) ? string.Empty : new FileInfo(action.File).FullName;
            var file = actionFile.Equals(string.Empty) ? new FileInfo(action.RenderedFile).FullName : actionFile;

            if (!file.Equals(string.Empty) && File.Exists(file)) {
                System.Diagnostics.Process.Start(file);
                Log.Info("Opened file {0}.", file);
            } else {
                Log.Warn("Can't open '{0}'.", file);
            }
        }
    }
}