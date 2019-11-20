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
using System;
using System.Collections.Generic;
using Cfg.Net;

namespace Transformalize.ConfigurationFacade {

   public class Connection : CfgNode {

      [Cfg]
      public string Name { get; set; }

      [Cfg]
      public string ConnectionString { get; set; }

      [Cfg]
      public string ContentType { get; set; }

      [Cfg]
      public string Database { get; set; }

      [Cfg]
      public string Core { get; set; }

      [Cfg]
      public string Cube { get; set; }

      [Cfg]
      public string Index { get; set; }

      [Cfg]
      public string DateFormat { get; set; }

      [Cfg]
      public string Delimiter { get; set; }

      [Cfg]
      public string Encoding { get; set; }

      [Cfg]
      public string End { get; set; }

      [Cfg]
      public string ErrorMode { get; set; }

      [Cfg]
      public string Folder { get; set; }

      [Cfg]
      public string File { get; set; }

      [Cfg]
      public string Footer { get; set; }

      [Cfg]
      public string Header { get; set; }

      [Cfg]
      public string Password { get; set; }

      [Cfg]
      public string Path { get; set; }

      [Cfg]
      public string Port { get; set; }

      [Cfg]
      public string Provider { get; set; }

      [Cfg]
      public string SearchOption { get; set; }

      [Cfg]
      public string SearchPattern { get; set; }

      [Cfg]
      public string Server { get; set; }

      [Cfg]
      public string Start { get; set; }

      [Cfg]
      public string Url { get; set; }

      [Cfg]
      public string User { get; set; }

      [Cfg]
      public string Version { get; set; }

      [Cfg]
      public string WebMethod { get; set; }

      [Cfg]
      public string RequestTimeout { get; set; }

      [Cfg]
      public string Timeout { get; set; }

      [Cfg]
      public string TextQualifier { get; set; }

      [Cfg]
      public string Schema { get; set; }

      [Cfg]
      public string Table { get; set; }

      [Cfg]
      public short MaxLength { get; set; }

      [Cfg]
      public short MinLength { get; set; }

      [Cfg]
      public short Sample { get; set; }

      [Cfg]
      public List<TflType> Types { get; set; }

      [Cfg]
      public List<Delimiter> Delimiters { get; set; }

      [Cfg]
      public List<Server> Servers { get; set; }

      [Cfg]
      public string SchemaFileName { get; set; }

      [Cfg]
      public string DropControl { get; set; }

      [Cfg]
      public string OpenWith { get; set; }

      [Cfg]
      public string Format { get; set; }

      [Cfg]
      public string ErrorLimit { get; set; }

      [Cfg]
      public string ModelType { get; set; }

      [Cfg]
      public string Command { get; set; }

      [Cfg]
      public string Arguments { get; set; }

      [Cfg]
      public string Template { get; set; }

      [Cfg]
      public string Stream { get; set; }

      [Cfg]
      public string Seed { get; set; }

      [Cfg]
      public string UseSsl { get; set; }

      [Cfg]
      public string ScrollWindow { get; set; }

      [Cfg]
      public string LinePattern { get; set; }

      [Cfg]
      public string Shards { get; set; }

      [Cfg]
      public string Replicas { get; set; }

      [Cfg]
      public string MaxDegreeOfParallism { get; set; }

      public Configuration.Connection ToConnection() {
         var map = new Configuration.Connection {
            Name = this.Name
         };
         return map;
      }

   }
}