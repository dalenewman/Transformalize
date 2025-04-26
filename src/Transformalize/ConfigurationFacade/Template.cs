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
using System.Collections.Generic;
using Cfg.Net;

namespace Transformalize.ConfigurationFacade {

    public class Template : CfgNode {

        [Cfg]
        public string Name { get; set; }
        [Cfg]
        public string ContentType { get; set; }
        [Cfg]
        public string File { get; set; }
        [Cfg]
        public string Cache { get; set; }
        [Cfg]
        public string Enabled { get; set; }

        [Cfg]
        public string Engine { get; set; }

        [Cfg]
        public List<Parameter> Parameters { get; set; }
        [Cfg]
        public List<Action> Actions { get; set; }

        [Cfg]
        public string Content { get; set; }

    }
}