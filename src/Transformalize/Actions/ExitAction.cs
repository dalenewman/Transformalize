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
using System.Threading;
using System.Threading.Tasks;
using Transformalize.Contracts;
using Action = Transformalize.Configuration.Action;

namespace Transformalize.Actions {

    public class ExitAction : IAction {

        private readonly Action _action;
        private readonly IContext _context;
        public ExitAction(IContext context, Action action) {
            _context = context;
            _action = action;
        }
        public ActionResponse Execute() {
            const string message = "Exiting TFL via action.";
            var response = new ActionResponse { Action = _action, Message = message };

            try {
                _context.Warn(message);
                _context.Logger.Clear();
#if NETS10
                _context.Warn("Unable to exit application.");
#else
                Environment.Exit(0);
#endif
            } catch (Exception ex) {
                response.Code = 500;
                response.Message = $"Exit failed: {ex.Message}";
            }
            return response;

        }

        public Task<ActionResponse> ExecuteAsync(CancellationToken cancellationToken = default) => Task.FromResult(Execute());
    }
}
