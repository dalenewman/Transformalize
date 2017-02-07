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
using System.Collections.Generic;
using Cfg.Net;

namespace Transformalize.Configuration {

    public class Template : CfgNode {

        [Cfg(required = true, unique = true, toLower = true)]
        public string Name { get; set; }
        [Cfg(value = "raw", domain = "raw,html", toLower = true, ignoreCase = true)]
        public string ContentType { get; set; }
        [Cfg(required = true, unique = true)]
        public string File { get; set; }
        [Cfg(value = false)]
        public bool Cache { get; set; }
        [Cfg(value = true)]
        public bool Enabled { get; set; }

        [Cfg(value = "razor", domain = "razor,velocity", toLower = true)]
        public string Engine { get; set; }

        [Cfg]
        public List<Parameter> Parameters { get; set; }
        [Cfg]
        public List<Action> Actions { get; set; }

        public string Key { get; set; }

        protected override void PostValidate() {
            var index = 0;
            foreach (var action in Actions) {
                action.InTemplate = true;
                action.Key = Name + (++index);
            }
        }
    }
}