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

using Autofac;
using Transformalize.Configuration;

namespace Transformalize.Providers.Elasticsearch.Autofac {

   public class ElasticsearchModule : Module {

      private Process _process;

      public ElasticsearchModule() { }
      public ElasticsearchModule(Process process) {
         _process = process;
      }

      protected override void Load(ContainerBuilder builder) {

         if (_process == null && builder.Properties.ContainsKey("Process")) {
            _process = (Process)builder.Properties["Process"];
         }

         if(_process == null) {
            return;
         }

         new ElasticsearchBuilder(_process, builder).Build();

      }
   }
}