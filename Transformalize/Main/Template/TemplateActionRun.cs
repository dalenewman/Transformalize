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
using System.Linq;
using Transformalize.Extensions;

namespace Transformalize.Main {
    public class TemplateActionRun : TemplateActionHandler {
        public override void Handle(TemplateAction action) {
            var fileInfo = new FileInfo(action.RenderedFile);
            if (fileInfo.Exists) {
                var script = File.ReadAllText(fileInfo.FullName);
                if (!string.IsNullOrEmpty(script)) {
                    var response = action.Connection.ExecuteScript(script);
                    if (response.Success) {
                        Log.Info("{0} ran successfully.", action.TemplateName);
                        Log.Debug("{0} affected {1} rows.", action.TemplateName,
                                  response.RowsAffected < 0 ? 0 : response.RowsAffected);
                    } else {
                        Log.Error("{0} failed. {1} Error{2}." + Environment.NewLine + string.Join(Environment.NewLine, response.Messages), action.TemplateName, response.Messages.Count, response.Messages.Count.Plural());
                        foreach (var message in response.Messages) {
                            Log.Warn(message);
                        }
                    }
                } else {
                    Log.Warn("{0} is empty.", action.TemplateName);
                }
            } else {
                Log.Warn("rendered output from {0} is not available.  It will not run.", action.TemplateName);
            }
        }
    }
}