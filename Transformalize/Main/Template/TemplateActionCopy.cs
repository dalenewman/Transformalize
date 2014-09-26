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
using System.Text.RegularExpressions;
using Transformalize.Main.Providers;
using Transformalize.Main.Providers.File;

namespace Transformalize.Main {

    public enum CopyType {
        FileToFile,
        FileToConnection,
        ConnectionToFile,
        ConnectionToConnection,
        Unknown
    }

    public class TemplateActionCopy : TemplateActionHandler {
        private readonly Process _process;

        public TemplateActionCopy(Process process) {
            _process = process;
        }

        private CopyType DeterminCopyType(string from, string to) {
            if (IsConnectionName(from)) {
                if (IsConnectionName(to)) {
                    return CopyType.ConnectionToConnection;
                }
                if (IsValidFileName(to)) {
                    return CopyType.ConnectionToFile;
                }
            } else if (IsValidFileName(from)) {
                {
                    if (IsConnectionName(to)) {
                        return CopyType.FileToConnection;
                    }
                    if (IsValidFileName(to)) {
                        return CopyType.FileToFile;
                    }
                }
            }
            return CopyType.Unknown;
        }


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

            var copyType = DeterminCopyType(from, to);
            if (copyType.Equals(CopyType.Unknown)) {
                TflLogger.Warn(action.ProcessName,string.Empty,"Unable to determine copy operation, from {0} to {1}.", from, to);
                TflLogger.Warn(action.ProcessName, string.Empty, "From must be a file name, a connection name, or blank to assume output from template.");
                TflLogger.Warn(action.ProcessName, string.Empty, "To must be a file name, or a connection name.");
                TflLogger.Warn(action.ProcessName, string.Empty, "Skipping {0} action.", action.Action);
                return;
            }

            FileInfo fromInfo;

            switch (copyType) {
                case CopyType.ConnectionToConnection:
                    break;
                case CopyType.ConnectionToFile:
                    break;
                case CopyType.FileToConnection:
                    fromInfo = new FileInfo(from);
                    if (fromInfo.Exists) {
                        var output = _process.Connections[to.ToLower()].Source;
                        var results = new FileImporter().Import(fromInfo, output);
                        TflLogger.Info(action.ProcessName, string.Empty, "Copied {0} to {1} connection.  Process name: {2}, Entity Name: {3}.", fromInfo.Name, to, results.Information.ProcessName, results.Information.EntityName);
                    } else {
                        TflLogger.Warn(action.ProcessName, string.Empty, "Unable to copy file {0}.  It may not exist.", fromInfo.Name);
                    }
                    break;
                default:
                    fromInfo = new FileInfo(from);
                    var toInfo = new FileInfo(to);
                    if (fromInfo.Exists && toInfo.Directory != null && toInfo.Directory.Exists) {
                        File.Copy(fromInfo.FullName, toInfo.FullName, true);
                        TflLogger.Info(action.ProcessName, string.Empty, "Copied {0} to {1}.", rendered ? action.TemplateName + " rendered output" : from, to);
                    } else {
                        TflLogger.Warn(action.ProcessName, string.Empty, "Unable to copy file {0} to folder {1}.  The folder may not exist.", fromInfo.Name, toInfo.DirectoryName);
                    }
                    break;
            }

        }

        private static bool IsValidFileName(string name) {
            var containsABadCharacter = new Regex("[" + Regex.Escape(string.Concat(Path.GetInvalidPathChars(), Path.GetInvalidFileNameChars())) + "]");
            if (containsABadCharacter.IsMatch(name)) {
                return false;
            };
            return name.Contains(".");
        }

        private bool IsConnectionName(string name) {
            return _process.Connections.ContainsKey(name.ToLower());
        }
    }
}