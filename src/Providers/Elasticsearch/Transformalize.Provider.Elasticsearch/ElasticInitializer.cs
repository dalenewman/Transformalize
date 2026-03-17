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

using System.Text.Json;
using Elastic.Transport;
using Transformalize.Actions;
using Transformalize.Context;
using Transformalize.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Transformalize.Providers.Elasticsearch {
   public class ElasticInitializer : IInitializer {

      private readonly OutputContext _context;
      private readonly ITransport _client;

      public ElasticInitializer(OutputContext context, ITransport client) {
         _context = context;
         _client = client;
      }

      public ActionResponse Execute() {

         var existsPath = new EndpointPath(HttpMethod.HEAD, $"/{_context.Connection.Index}");
         if (_client.Request<DynamicResponse>(in existsPath).ApiCallDetails.HttpStatusCode == 200) {
            var deletePath = new EndpointPath(HttpMethod.DELETE, $"/{_context.Connection.Index}");
            _client.Request<DynamicResponse>(in deletePath);
         }

         var settingsObj = new { settings = new { number_of_shards = _context.Connection.Shards, number_of_replicas = _context.Connection.Replicas } };
         var settingsJson = JsonSerializer.Serialize(settingsObj);
         var createPath = new EndpointPath(HttpMethod.PUT, $"/{_context.Connection.Index}");
         var elasticResponse = _client.Request<DynamicResponse>(in createPath, PostData.String(settingsJson));

         var response = new ActionResponse(
            (int?)elasticResponse.ApiCallDetails?.HttpStatusCode ?? 500,
            elasticResponse.ApiCallDetails?.OriginalException == null ? string.Empty : (elasticResponse.ApiCallDetails?.DebugInformation ?? string.Empty).Replace("{", "{{").Replace("}", "}}")
         ) {
            Action = new Configuration.Action() {
               Type = "internal",
               ErrorMode = "continue",
               Description = "Elasticsearch Initializer"
            }
         };

         return response;
      }

   public async Task<ActionResponse> ExecuteAsync(CancellationToken token = default) {

         var existsPath = new EndpointPath(HttpMethod.HEAD, $"/{_context.Connection.Index}");
         var existsResponse = await _client.RequestAsync<DynamicResponse>(in existsPath, null, token).ConfigureAwait(false);
         if (existsResponse.ApiCallDetails.HttpStatusCode == 200) {
            var deletePath = new EndpointPath(HttpMethod.DELETE, $"/{_context.Connection.Index}");
            await _client.RequestAsync<DynamicResponse>(in deletePath, null, token).ConfigureAwait(false);
         }

         var settingsObj2 = new { settings = new { number_of_shards = _context.Connection.Shards, number_of_replicas = _context.Connection.Replicas } };
         var settingsJson2 = JsonSerializer.Serialize(settingsObj2);
         var createPath = new EndpointPath(HttpMethod.PUT, $"/{_context.Connection.Index}");
         var elasticResponse = await _client.RequestAsync<DynamicResponse>(in createPath, PostData.String(settingsJson2), token).ConfigureAwait(false);

         var response = new ActionResponse(
            (int?)elasticResponse.ApiCallDetails?.HttpStatusCode ?? 500,
            elasticResponse.ApiCallDetails?.OriginalException == null ? string.Empty : (elasticResponse.ApiCallDetails?.DebugInformation ?? string.Empty).Replace("{", "{{").Replace("}", "}}")
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
