#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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
using System.Threading;
using System.Threading.Tasks;
using Transformalize.Actions;
using Transformalize.Contracts;
using Action = Transformalize.Configuration.Action;

namespace Transformalize.Providers.File.Actions {
    public class FileCopyAction : IAction {

        private readonly IContext _context;
        private readonly Action _action;

        public FileCopyAction(IContext context, Action action) {
            _context = context;
            _action = action;
        }

        public ActionResponse Execute() {
            string message;
            int status;
            var from = new FileInfo(_action.From);
            var to = new FileInfo(_action.To);

            _context.Info("Copying {0} to {1}", from.Name, to.Name);

            try {
                System.IO.File.Copy(from.FullName, to.FullName, true);
                status = 200;
                message = $"Copied file from {from.Name} to {to.Name}.";
            } catch (Exception ex) {
                _context.Error(ex.Message);
                status = 500;
                message = ex.Message;
            }

            return new ActionResponse(status, message) { Action = _action };
        }

        public Task<ActionResponse> ExecuteAsync(CancellationToken cancellationToken = default) => Task.FromResult(Execute());
    }
}