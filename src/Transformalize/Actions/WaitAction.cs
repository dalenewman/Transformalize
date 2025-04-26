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
    public class WaitAction : IAction {

        private readonly Action _action;
        public WaitAction(Action action) {
            _action = action;
        }

        public ActionResponse Execute() {
            var message = _action.Type == "wait" ? $"Waited for {_action.TimeOut} ms" : $"Slept for {_action.TimeOut} ms";
#if NETS10
            System.Threading.Tasks.Task.Delay(_action.TimeOut).Wait();
#else
            System.Threading.Thread.Sleep(_action.TimeOut);
#endif
            return new ActionResponse(200, message) { Action = _action };
        }
    }
}