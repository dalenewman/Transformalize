#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
using System.Collections.Generic;
using System.Linq;
using Pipeline.Contracts;

namespace Pipeline {

    public class ProcessController : IProcessController {

        private readonly IEnumerable<IPipeline> _pipelines;
        private readonly IContext _context;
        public List<IAction> PreActions { get; } = new List<IAction>();
        public List<IAction> PostActions { get; } = new List<IAction>();

        public ProcessController(
            IEnumerable<IPipeline> pipelines,
            IContext context
        ) {
            _pipelines = pipelines;
            _context = context;
        }

        private bool PreExecute() {
            foreach (var action in PreActions) {
                _context.Debug(() => $"Pre-Executing {action.GetType().Name}");
                var response = action.Execute();
                if (response.Code == 200)
                    continue;
                _context.Error(response.Content);
                return false;
            }
            return true;
        }

        public void Execute() {
            if (PreExecute()) {
                foreach (var entity in _pipelines) {
                    _context.Debug(() => $"Initializing {entity.GetType().Name}");
                    entity.Initialize();
                }
                foreach (var entity in _pipelines) {
                    _context.Debug(() => $"Executing {entity.GetType().Name}");
                    entity.Execute();
                }
                PostExecute();
            } else {
                _context.Error("Pre-Execute failed. Abort!");
            }
        }

        private void PostExecute() {
            foreach (var action in PostActions) {
                _context.Debug(() => $"Post-Executing {action.GetType().Name}");
                var response = action.Execute();
                if (response.Code != 200) {
                    _context.Error(response.Content);
                }
            }
        }

        public IEnumerable<IRow> Read() {
            // todo, flatten and send all entity data back, for now, take first entity
            return _pipelines.First().Read();
        }

        public void Dispose() {
            PreActions.Clear();
            PostActions.Clear();
            foreach (var pipeline in _pipelines) {
                pipeline.Dispose();
            }
        }
    }
}
