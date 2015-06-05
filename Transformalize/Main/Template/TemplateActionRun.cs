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
using Transformalize.Logging;

namespace Transformalize.Main {

    public class TemplateActionRun : TemplateActionHandler {
        private readonly ILogger _logger;

        public TemplateActionRun(ILogger logger)
        {
            _logger = logger;
        }

        public override void Handle(TemplateAction action) {

            if (action.Connection == null) {
                _logger.Warn("No connection provided for {0}", action.Action);
                return;
            }

            if (!string.IsNullOrEmpty(action.Command)) {
                var response = action.Connection.ExecuteScript(action.Command, action.Timeout);
                if (response.Success) {
                    _logger.Info("Command: {0} ran ok.", action.Command.Left(20)+"...");
                    _logger.Debug("Command: {0} affected {1} rows.", action.Command, response.RowsAffected < 0 ? 0 : response.RowsAffected);
                } else {
                    _logger.Error("Failed to run command: {0}. {1} Error{2}.", action.Command, response.Messages.Count, response.Messages.Count.Plural());
                    foreach (var message in response.Messages) {
                        _logger.Error(message);
                    }
                }
                return;
            }

            var isTemplate = string.IsNullOrEmpty(action.File);

            var file = isTemplate ? action.RenderedFile : action.File;

            if (string.IsNullOrEmpty(file) && string.IsNullOrEmpty(action.Command)) {
                _logger.Warn("Skipping run action.  It needs a template file, OR an action file attribute set.  Note: the action file superceeds the template file.");
                return;
            }

            var fileInfo = new FileInfo(file);
            var fileExists = fileInfo.Exists;

            if (fileExists) {
                var script = File.ReadAllText(fileInfo.FullName);
                if (!string.IsNullOrEmpty(script)) {

                    var response = action.Connection.ExecuteScript(script, action.Timeout);
                    if (response.Success) {
                        _logger.Info("{0} ran successfully.", fileInfo.Name);
                        _logger.Debug("{0} affected {1} rows.", fileInfo.Name, response.RowsAffected < 0 ? 0 : response.RowsAffected);
                    } else {
                        _logger.Error("{0} failed. {1} Error{2}.", fileInfo.Name, response.Messages.Count, response.Messages.Count.Plural());
                        foreach (var message in response.Messages) {
                            _logger.Error(message);
                        }
                    }
                } else {
                    _logger.Warn("{0} is empty.", fileInfo.Name);
                }
            } else {
                if (isTemplate)
                    _logger.Warn("file rendered output from {0} is not available.  It will not run.", action.TemplateName);
                else
                    _logger.Warn("file {0} output is not available.  It will not run.", fileInfo.Name);
            }
        }

    }
}