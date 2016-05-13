#region license
// Transformalize
// Copyright 2013 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System.Collections.Generic;
using Cfg.Net;

namespace Pipeline.Configuration {

    public class Map : CfgNode {

        [Cfg(required = true, unique = true, toLower = true)]
        public string Name { get; set; }

        [Cfg(value = "input", toLower = true)]
        public string Connection { get; set; }

        [Cfg(value = "")]
        public string Query { get; set; }

        [Cfg(required = false)]
        public List<MapItem> Items { get; set; }

        protected override void Validate() {
            if (Items.Count == 0 && Query == string.Empty) {
                Error($"Map '{Name}' needs items or a query.");
            }
        }

        public string Key { get; set; }
    }
}