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
using Cfg.Net;
using System.Collections.Generic;
using System.Linq;

namespace Transformalize.ConfigurationFacade {

    public class Map : CfgNode {

        [Cfg]
        public string Name { get; set; }

        [Cfg]
        public string PassThrough { get; set; }

        [Cfg]
        public string Connection { get; set; }

        [Cfg]
        public string Query { get; set; }

        [Cfg]
        public List<MapItem> Items { get; set; }

        public Configuration.Map ToMap() {
            var map = new Configuration.Map {
                Name = this.Name,
                Connection = this.Connection,
                Query = this.Query,
                Items = this.Items.Select(mi => mi.ToMapItem()).ToList()
            };

            bool.TryParse(this.PassThrough, out bool passThrough);
            map.PassThrough = passThrough;

            return map;
        }

    }
}