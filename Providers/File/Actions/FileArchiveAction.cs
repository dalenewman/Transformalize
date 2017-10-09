#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.IO;
using Transformalize.Actions;
using Transformalize.Contracts;
using Action = Transformalize.Configuration.Action;

namespace Transformalize.Providers.File.Actions {
    public class FileArchiveAction : IAction {

        private readonly IContext _context;
        private readonly Action _action;

        public FileArchiveAction(IContext context, Action action) {
            _context = context;
            _action = action;
        }

        public ActionResponse Execute() {

            var from = new FileInfo(_action.From); // a file
            var to = new FileInfo(_action.To); // a folder

            if (!Path.HasExtension(to.FullName)) {
                // give it same name as from file name
                var date = DateTime.UtcNow;
                var fromName = $"{Path.GetFileNameWithoutExtension(from.Name)}-{date:yyyy-MM-dd-HH-mm-ss-fff}{Path.GetExtension(from.Name)}";

                var year = date.Year.ToString();
                var month = $"{date:MM-MMM}".ToUpper();

                to = new FileInfo(Path.Combine(to.FullName, year, month, fromName));
            }

            _context.Info("Archiving {0} from {1} to {2}", from.Name, from.DirectoryName, to.DirectoryName);
            try {
                Directory.CreateDirectory(to.DirectoryName);
                System.IO.File.Move(from.FullName, to.FullName);
                return new ActionResponse(200, $"Archived {from.Name} from {from.DirectoryName} to {to.DirectoryName}") { Action = _action };
            } catch (Exception ex) {
                return new ActionResponse(500, ex.Message) { Action = _action };
            }
        }
    }
}