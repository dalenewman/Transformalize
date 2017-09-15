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
using System.IO;
using Transformalize.Actions;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Providers.File.Actions {
    public class FileReplaceAction : IAction {
        private readonly IContext _context;
        private readonly Action _action;
        private readonly FileInfo _fileInfo;

        public FileReplaceAction(IContext context, Action action) {
            _context = context;
            _action = action;
            _fileInfo = new FileInfo(_action.File);
        }

        public ActionResponse Execute() {
            var response = new ActionResponse { Action = _action };
            if (_fileInfo.Exists) {
                var content = System.IO.File.ReadAllText(_fileInfo.FullName);
                System.IO.File.WriteAllText(_fileInfo.FullName, content.Replace(_action.OldValue, _action.NewValue));
                _context.Info($"Replaced {_action.OldValue} with {_action.NewValue} in {_action.File}");
            } else {
                response.Code = 401;
                response.Message = $"File {_action.File} not found";
            }
            return response;
        }
    }
}