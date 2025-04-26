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
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Actions {

    public class LogAction : IAction {

        private readonly IContext _context;
        private readonly Action _action;
        private readonly char _level;

        public LogAction(IContext context, Action action) {
            _context = context;
            _action = action;
            _level = action.Level[0];
        }

        public ActionResponse Execute() {
            switch (_level) {
                case 'e':
                    _context.Error(_action.Message);
                    break;
                case 'd':
                    _context.Debug((() => _action.Message));
                    break;
                case 'w':
                    _context.Warn(_action.Message);
                    break;
                default:
                    _context.Info(_action.Message);
                    break;
            }
            return new ActionResponse(200, _action.Message) { Action = _action };
        }
    }
}