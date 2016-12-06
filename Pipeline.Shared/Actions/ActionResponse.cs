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

using Cfg.Net.Ext;
using Transformalize.Configuration;

namespace Transformalize.Actions {

    public class ActionResponse {

        public string Message { get; set; } = string.Empty;
        public int Code { get; set; } = 200;
        public Action Action { get; set; }

        public ActionResponse() {
            Action = new Action { Type = "internal" }.WithDefaults();
        }

        public ActionResponse(string message) {
            Action = new Action { Type = "internal" }.WithDefaults();
            Message = message;
        }

        public ActionResponse(int code) {
            Action = new Action { Type = "internal" }.WithDefaults();
            Code = code;
        }

        public ActionResponse(int code, string message) {
            Action = new Action { Type = "internal" }.WithDefaults();
            Code = code;
            Message = message;
        }

    }
}
