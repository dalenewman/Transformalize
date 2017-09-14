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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Transformalize.Contracts;

namespace Transformalize {

    public class ProcessController : IProcessController {

        private readonly IEnumerable<IPipeline> _pipelines;
        private readonly IContext _context;
        public List<IAction> PreActions { get; } = new List<IAction>();
        public List<IAction> PostActions { get; } = new List<IAction>();
        private Stopwatch _stopwatch = new Stopwatch();
        private readonly bool _valid;

        public ProcessController(
            IEnumerable<IPipeline> pipelines,
            IContext context
        ) {
            _stopwatch.Start();
            _pipelines = pipelines;
            _context = context;
            _valid = _pipelines.All(p => p.Valid);
        }

        private bool PreExecute() {
            if (!_valid) {
                _context.Error("The pipelines have invalid transforms.  They must be fixed before the process can run.");
                return false;
            }

            foreach (var action in PreActions) {
                _context.Debug(() => $"Pre-Executing {action.GetType().Name}");
                var response = action.Execute();

                if (response.Code == 200) {
                    if (response.Action.Type != "internal") {
                        _context.Info($"Successfully ran pre-action {response.Action.Type}.");
                    }
                    if (response.Message != string.Empty) {
                        _context.Debug(() => response.Message);
                    }
                    continue;
                }

                var errorMode = response.Action.ToErrorMode();

                if (errorMode == ErrorMode.Default || errorMode == ErrorMode.Abort) {
                    _context.Error("Abort: " + response.Message);
                    return false;
                }

                if (errorMode == ErrorMode.Continue) {
                    _context.Error("Continue: " + response.Message);
                    continue;
                }

                if (errorMode == ErrorMode.Exception) {
                    _context.Error("Exception: " + response.Message);
                    throw new Exception(response.Message);
                }
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

                if (response.Code == 200) {
                    if (response.Action.Type != "internal") {
                        _context.Info($"Successfully ran post-action {response.Action.Type}.");
                    }
                    if (response.Message != string.Empty) {
                        _context.Debug(() => response.Message);
                    }
                    continue;
                }

                var errorMode = response.Action.ToErrorMode();

                if (errorMode == ErrorMode.Default || errorMode == ErrorMode.Continue) {
                    _context.Error("Continue: " + response.Message);
                    continue;
                }

                if (errorMode == ErrorMode.Abort) {
                    _context.Error("Abort: " + response.Message);
                    break;
                }

                if (errorMode == ErrorMode.Exception) {
                    _context.Error("Exception: " + response.Message);
                    throw new Exception(response.Message);
                }

            }
        }

        public IEnumerable<IRow> Read() {
            foreach (var pl in _pipelines) {
                foreach (var row in pl.Read()) {
                    yield return row;
                }
            };
        }

        public void Dispose() {
            PreActions.Clear();
            PostActions.Clear();
            foreach (var pipeline in _pipelines) {
                pipeline.Dispose();
            }
            _stopwatch.Stop();
            _context.Info($"Time elapsed: {_stopwatch.Elapsed}");
            _stopwatch = null;
        }
    }
}
