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
using Cfg.Net;

namespace Transformalize.ConfigurationFacade {
    public class Filter : CfgNode {

        [Cfg]
        public string Continuation { get; set; }

        [Cfg]
        public string Expression { get; set; }

        [Cfg]
        public string Field { get; set; }

        [Cfg]
        public string Value { get; set; }

        [Cfg]
        public string Operator { get; set; }

        [Cfg]
        public string Type { get; set; }

        [Cfg]
        public string Size { get; set; }

        [Cfg]
        public string OrderBy { get; set; }

        [Cfg]
        public string Order { get; set; }

        [Cfg]
        public string Min { get; set; }

        [Cfg]
        public string Map { get; set; }

    }
}