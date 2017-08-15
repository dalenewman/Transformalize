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
using Cfg.Net.Contracts;
using Cfg.Net.Loggers;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Contracts;

namespace Transformalize.Actions {

    public class ScriptLoaderAction : IAction {

        readonly IContext _context;
        readonly IReader _reader;

        public ScriptLoaderAction(IContext context, IReader reader) {
            _context = context;
            _reader = reader;
        }

        public ActionResponse Execute() {
            var logger = new MemoryLogger();
            var code = 200;
            var message = "Ok";
            foreach (var script in _context.Process.Scripts) {
                if (script.File != string.Empty) {
                    script.Content = _reader.Read(script.File, new Dictionary<string,string>() , logger);
                    if (logger.Errors().Any()) {
                        code = 500;
                        foreach (var error in logger.Errors()) {
                            message += error + " ";
                        }
                        break;
                    }
                    foreach (var entity in _context.Process.Entities) {
                        if (entity.Script == script.Name && entity.Query == string.Empty && script.Content != string.Empty) {
                            entity.Query = script.Content;
                        }
                    }
                }
            }

            return new ActionResponse(code, message) {
                Action = new Configuration.Action {
                    Before = true,
                    After = false,
                    Type = "internal",
                    ErrorMode = "abort",
                    Description = "load script"
                }
            };
        }
    }
}