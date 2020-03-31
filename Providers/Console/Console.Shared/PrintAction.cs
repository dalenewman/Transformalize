#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
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
using Transformalize.Actions;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Action = Transformalize.Configuration.Action;

namespace Transformalize.Providers.Console {
    public class PrintAction : IAction {
        private readonly Action _action;
        private readonly string _level;

        public PrintAction(Action action) {
            _action = action;
            _level = action.Level.Left(1);
        }

        public ActionResponse Execute() {

            if (_level == "e") {
                System.Console.Error.WriteLine(_action.Message);
            } else {
                System.Console.WriteLine(_action.Message);
            }
            return new ActionResponse(200, _action.Message) { Action = _action };
        }
    }
}