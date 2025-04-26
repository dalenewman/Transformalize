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
using Transformalize.Configuration;

namespace Transformalize {

   public class ActionResponse {

      public int Code { get; set; } = 200;
      public string Message { get; set; } = string.Empty;
      public byte[] Data { get; set; }
      public Action Action { get; set; }

      public ActionResponse() {
         Action = new Action { Type = "internal" };
      }

      public ActionResponse(string message) {
         Action = new Action { Type = "internal" };
         Message = message;
      }

      public ActionResponse(int code) {
         Action = new Action { Type = "internal" };
         Code = code;
      }

      public ActionResponse(int code, string message) {
         Action = new Action { Type = "internal" };
         Code = code;
         Message = message;
      }

   }
}
