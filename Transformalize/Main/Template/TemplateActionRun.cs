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
using Transformalize.Extensions;

namespace Transformalize.Main {

    public class TemplateActionRun : TemplateActionHandler {

        public override void Handle(TemplateAction action) {

            var isTemplate = string.IsNullOrEmpty(action.File);
            var file = isTemplate ? action.RenderedFile : action.File;

            if (string.IsNullOrEmpty(file)) {
                Log.Warn("Skipping run action.  It needs a template file, OR an action file attribute set.  Note: the action file superceeds the template file.");
                return;
            }

            var fileInfo = new FileInfo(file);

            if (fileInfo.Exists) {
                var script = File.ReadAllText(fileInfo.FullName);
                if (!string.IsNullOrEmpty(script)) {

                    if (action.Connection == null) {
                        if (isTemplate) {
                            Log.Warn("Could not run {0} action for {1} template.  No connection provided.", action.Action, action.TemplateName);
                        } else {
                            Log.Warn("Could not run {0} action.  No connection provided.", action.Action);
                        }
                        return;
                    }

                    var response = action.Connection.ExecuteScript(script, action.Timeout);
                    if (response.Success) {
                        Log.Info("{0} ran successfully.", fileInfo.Name);
                        Log.Debug("{0} affected {1} rows.", fileInfo.Name, response.RowsAffected < 0 ? 0 : response.RowsAffected);
                    } else {
                        Log.Error("{0} failed. {1} Error{2}.", fileInfo.Name, response.Messages.Count, response.Messages.Count.Plural());
                        foreach (var message in response.Messages) {
                            Log.Error(message);
                        }
                    }
                } else {
                    Log.Warn("{0} is empty.", fileInfo.Name);
                }
            } else {
                if(isTemplate)
                    Log.Warn("file rendered output from {0} is not available.  It will not run.", action.TemplateName);
                else
                    Log.Warn("file {0} output is not available.  It will not run.", fileInfo.Name);
            }
        }
    }
}