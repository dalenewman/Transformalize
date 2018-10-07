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

namespace Transformalize.ConfigurationFacade {

    public class Action : CfgNode {

        [Cfg]
        public string Type { get; set; }

        [Cfg]
        public string After { get; set; }

        [Cfg]
        public string ErrorMode { get; set; }

        [Cfg]
        public string Arguments { get; set; }

        [Cfg]
        public string Bcc { get; set; }

        [Cfg]
        public string Before { get; set; }

        [Cfg]
        public string Cc { get; set; }

        [Cfg]
        public string Command { get; set; }

        [Cfg]
        public string Connection { get; set; }

        [Cfg]
        public string File { get; set; }

        [Cfg]
        public string From { get; set; }

        [Cfg]
        public string Html { get; set; }

        [Cfg]
        public string Method { get; set; }

        [Cfg]
        public string Mode { get; set; }

        [Cfg]
        public string NewValue { get; set; }

        [Cfg]
        public string OldValue { get; set; }

        [Cfg]
        public string Subject { get; set; }

        [Cfg]
        public string TimeOut { get; set; }

        [Cfg]
        public string To { get; set; }

        [Cfg]
        public string Url { get; set; }

        [Cfg]
        public string Id { get; set; }

        [Cfg]
        public string Body { get; set; }

        [Cfg]
        public string PlaceHolderStyle { get; set; }

        [Cfg]
        public List<NameReference> Modes { get; set; }

        [Cfg]
        public string Level { get; set; }

        [Cfg]
        public string Message { get; set; }

        [Cfg]
        public string RowCount { get; set; }

        [Cfg]
        public string Description { get; set; }
    }
}