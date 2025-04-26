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

namespace Transformalize.ConfigurationFacade {
   public class Server : CfgNode {
      [Cfg]
      public string Url { get; set; }

      [Cfg]
      public string Name { get; set; }

      [Cfg]
      public string Port { get; set; }

      [Cfg]
      public string Path { get; set; }

      public Configuration.Server ToServer() {
         var server = new Configuration.Server() {
            Url = this.Url,
            Name = this.Name,
            Path = this.Path
         };

         int.TryParse(this.Port, out int port);
         server.Port = port;

         return server;
      }

   }
}