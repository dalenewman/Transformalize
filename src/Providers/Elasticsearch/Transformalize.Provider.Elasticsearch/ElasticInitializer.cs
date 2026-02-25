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

using Elasticsearch.Net;
using Newtonsoft.Json.Linq;
using Transformalize.Actions;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.Elasticsearch {
   public class ElasticInitializer : IInitializer {

      private readonly OutputContext _context;
      private readonly IElasticLowLevelClient _client;

      public ElasticInitializer(OutputContext context, IElasticLowLevelClient client) {
         _context = context;
         _client = client;
      }

      public ActionResponse Execute() {

         if (_client.Indices.Exists<DynamicResponse>(_context.Connection.Index).HttpStatusCode == 200) {
            _client.Indices.Delete<VoidResponse>(_context.Connection.Index);
         }

         var settings = new JObject { { "settings", new JObject { { "number_of_shards", _context.Connection.Shards }, { "number_of_replicas", _context.Connection.Replicas } } } };
         var elasticResponse = _client.Indices.Create<DynamicResponse>(_context.Connection.Index, settings.ToString());

         var response = new ActionResponse(
            elasticResponse.HttpStatusCode ?? 500,
            elasticResponse.OriginalException == null ? string.Empty : elasticResponse.DebugInformation.Replace("{", "{{").Replace("}", "}}") ?? string.Empty
         ) {
            Action = new Configuration.Action() {
               Type = "internal",
               ErrorMode = "continue",
               Description = "Elasticsearch Initializer"
            }
         };

         return response;
      }
   }
}