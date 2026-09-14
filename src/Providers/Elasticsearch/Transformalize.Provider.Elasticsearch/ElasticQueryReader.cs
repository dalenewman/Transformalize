using System.Text.Json;
using Transformalize.Extensions;
using System.Runtime.CompilerServices;
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
using System.Linq;
using Elastic.Transport;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Transformalize.Providers.Elasticsearch {

    public class ElasticQueryReader : IReadStream, IRead {

        readonly ITransport _client;
        private readonly IRowFactory _rowFactory;
        readonly InputContext _context;
        private readonly Dictionary<string, Field> _fields;
        private readonly HashSet<string> _missing = new HashSet<string>();

        public ElasticQueryReader(
            InputContext context,
            ITransport client,
            IRowFactory rowFactory
        ) {
            _context = context;
            _client = client;
            _rowFactory = rowFactory;
            _fields = context.InputFields.ToDictionary(k => k.Name, v => v);
        }

        public IEnumerable<IRow> Read() {
            _context.Debug(() => _context.Entity.Query);
            var searchPath = new EndpointPath(HttpMethod.POST, "/_search");
            var response = _client.Request<DynamicResponse>(in searchPath, PostData.String(_context.Entity.Query));

            if (response.ApiCallDetails.HasSuccessfulStatusCode) {
                if (response.Body != null && response.Body["aggregations"].HasValue) {
                    return Flatten("aggregations", response.Body["aggregations"].Value);
                }
                _context.Warn("An elastic query should return aggregations, but yours does not.");
            } else {
                _context.Error(response.ApiCallDetails.DebugInformation.Replace("{", "{{").Replace("}", "}}"));
            }
            return new IRow[0];
        }

        private IEnumerable<IRow> Flatten(string key, object obj) {
            var results = new List<IRow>();
            var state = new FlattenState();
            foreach (var row in FlattenStream(key, obj, state, CancellationToken.None)) results.Add(row);
            if (state.Row != null) results.Add(state.Row);
            foreach (var missing in _missing) _context.Warn($"The query returns field {missing}, but you do not have that field defined in {_context.Entity.Alias}.");
            return results;
        }

        public async IAsyncEnumerable<IRow> ReadStreamAsync([EnumeratorCancellation] CancellationToken token = default) {
         token.ThrowIfCancellationRequested();
         _context.Debug(() => _context.Entity.Query);
         var path = new EndpointPath(HttpMethod.POST, "/_search");
         var response = await _client.RequestAsync<DynamicResponse>(in path, PostData.String(_context.Entity.Query), token).ConfigureAwait(false);
         token.ThrowIfCancellationRequested();
         if (!response.ApiCallDetails.HasSuccessfulStatusCode) {
            _context.Error(response.ApiCallDetails.DebugInformation.Replace("{", "{{").Replace("}", "}}"));
            throw new System.InvalidOperationException("Elasticsearch aggregation query failed.");
         }
         if (response.Body == null || !response.Body["aggregations"].HasValue) {
            _context.Warn("An elastic query should return aggregations, but yours does not.");
            yield break;
         }
         var state = new FlattenState();
         foreach (var row in FlattenStream("aggregations", response.Body["aggregations"].Value, state, token)) {
            yield return row;
         }
         if (state.Row != null) yield return state.Row;
         foreach (var missing in _missing) _context.Warn($"The query returns field {missing}, but you do not have that field defined in {_context.Entity.Alias}.");
      }

      private static object UnwrapValue(object value) {
         return value is JsonElement element ? DynamicValue.ConsumeJsonElement(typeof(object), element) : value;
      }

      private sealed class FlattenState { public IRow Row; }

      private IEnumerable<IRow> FlattenStream(string key, object obj, FlattenState state, CancellationToken token) {
         token.ThrowIfCancellationRequested();
         _fields.TryGetValue(key, out var field);
         // Elastic.Transport exposes JsonElement nodes. Expand only this node;
         // keep bucket arrays lazy instead of converting the complete response tree.
         IEnumerable<object> list = obj as IList<object>;
         if (obj is JsonElement element) {
            if (element.ValueKind == JsonValueKind.Array) list = element.EnumerateArray().Select(item => (object)item);
            else if (element.ValueKind == JsonValueKind.Object) obj = element.EnumerateObject().ToDictionary(p => p.Name, p => (object)p.Value);
         }
         if (list != null) {
            foreach (var item in list) {
               if (state.Row != null) yield return state.Row;
               state.Row = _rowFactory.Create();
               foreach (var row in FlattenStream(key, item, state, token)) yield return row;
            }
         } else if (obj is IDictionary<string, object> dict) {
            if (dict.Count == 1 && dict.ContainsKey("value")) {
               if (field == null) _missing.Add(key);
               else {
                  if (state.Row != null && state.Row[field] != null) { yield return state.Row; state.Row = null; }
                  if (state.Row == null) state.Row = _rowFactory.Create();
                  state.Row[field] = UnwrapValue(dict["value"]);
               }
            } else if (dict.ContainsKey("buckets")) {
               foreach (var row in FlattenStream(key, dict["buckets"], state, token)) yield return row;
            } else {
               var bucket = dict.ContainsKey("key") && dict.ContainsKey("doc_count");
               foreach (var pair in dict) {
                  if (bucket && pair.Key == "key") {
                     if (field == null) _missing.Add(key);
                     else {
                        if (state.Row == null) state.Row = _rowFactory.Create();
                        state.Row[field] = UnwrapValue(pair.Value);
                     }
                  } else if (!bucket || pair.Key != "doc_count") {
                     foreach (var row in FlattenStream(pair.Key, pair.Value, state, token)) yield return row;
                  }
               }
            }
         }
      }


        public async Task<IEnumerable<IRow>> ReadAsync(CancellationToken token = default) {
            _context.Debug(() => _context.Entity.Query);
            var asyncSearchPath = new EndpointPath(HttpMethod.POST, "/_search");
            var response = await _client.RequestAsync<DynamicResponse>(in asyncSearchPath, PostData.String(_context.Entity.Query), token).ConfigureAwait(false);

            if (response.ApiCallDetails.HasSuccessfulStatusCode) {
                if (response.Body != null && response.Body["aggregations"].HasValue) {
                    return Flatten("aggregations", response.Body["aggregations"].Value);
                }
                _context.Warn("An elastic query should return aggregations, but yours does not.");
            } else {
                _context.Error(response.ApiCallDetails.DebugInformation.Replace("{", "{{").Replace("}", "}}"));
            }
            return new IRow[0];
        }
    }
}
