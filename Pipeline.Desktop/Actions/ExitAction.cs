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
using Transformalize.Actions;
using Transformalize.Contracts;
using Action = Transformalize.Configuration.Action;

namespace Transformalize.Desktop.Actions {
    public class ExitAction : IAction {

        private readonly Action _action;
        private readonly IContext _context;
        public ExitAction(IContext context, Action action) {
            _context = context;
            _action = action;
        }
        public ActionResponse Execute() {
            var response = new ActionResponse { Action = _action };
            try {
                _context.Warn("Exit Action is shutting TFL down.");
                _context.Logger.Clear();
                Environment.Exit(0);
            } catch (Exception ex) {
                response.Code = 500;
                response.Message = $"Exit failed: {ex.Message}";
            }
            return response;

        }
    }
}
