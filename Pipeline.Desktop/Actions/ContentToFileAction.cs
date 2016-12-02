#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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
using Pipeline.Contracts;
using System.IO;
using System.Runtime.CompilerServices;
using Pipeline.Context;
using Action = Pipeline.Configuration.Action;

namespace Pipeline.Desktop.Actions {
    public class ContentToFileAction : IAction {
        private readonly PipelineContext _context;
        private readonly Action _action;

        public ContentToFileAction(PipelineContext context, Action action) {
            _context = context;
            _action = action;
        }

        public ActionResponse Execute() {
            var response = new ActionResponse { Action = _action };
            var to = new FileInfo(_action.To);
            if (string.IsNullOrEmpty(_action.Body)) {
                _context.Warn("Nothing to write to {0}", to.Name);
            } else {
                _context.Info("Writing content to {0}", to.Name);
                try {
                    File.WriteAllText(to.FullName, _action.Body);
                } catch (Exception ex) {
                    _context.Error($"Failed to run {_action.Type} action.");
                    _context.Error(ex.Message);
                    response.Code = 500;
                }
            }
            return response;
        }
    }
}
