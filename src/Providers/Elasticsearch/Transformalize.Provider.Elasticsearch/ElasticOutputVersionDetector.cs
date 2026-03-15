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

using System;
using System.Collections.Generic;
using System.Text.Json;
using Elastic.Transport;
using Newtonsoft.Json;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Providers.Elasticsearch.Ext;
using System.Threading;
using System.Threading.Tasks;

namespace Transformalize.Providers.Elasticsearch {
   public class ElasticOutputProvider : IOutputProvider {

      private readonly OutputContext _context;
      private readonly ITransport _client;
      private DynamicResponse _commonAggregations;

      public ElasticOutputProvider(OutputContext context, ITransport client) {
         _context = context;
         _client = client;
      }

      public void Delete() {
         throw new NotImplementedException();
      }

      public object GetMaxVersion() {

         // TODO: Consider tlfdeleted = 0

         if (string.IsNullOrEmpty(_context.Entity.Version))
            return null;

         var version = _context.Entity.GetVersionField();

         _context.Debug(() => $"Detecting Max Output Version: {_context.Connection.Index}.{_context.TypeName()}.{version.Alias.ToLower()}.");

         var body = new {
            aggs = new {
               version = new {
                  max = new {
                     field = version.Alias.ToLower()
                  }
               }
            },
            size = 0
         };

         var searchPath = new EndpointPath(HttpMethod.POST, $"/{_context.Connection.Index}/_search");
         var result = _client.Request<DynamicResponse>(in searchPath, PostData.String(JsonConvert.SerializeObject(body)));
         dynamic converted = null;
         if (result.ApiCallDetails.HasSuccessfulStatusCode) {
            try {
               var dynValue = result.Body["aggregations"]["version"]["value"];
               if (dynValue.HasValue) {
                  if (version.Type.StartsWith("date")) {
                     try {
                        var ms = DynamicToLong(dynValue);
                        converted = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(ms);
                     } catch (Exception e) {
                        _context.Error(e, $"Failed converting {version.Name} value to long (for epoch_millis) conversion to datetime.");
                     }
                  } else {
                     converted = DynamicToObject(dynValue);
                  }
               }
            } catch (Exception ex) {
               _context.Error(ex, ex.Message);
            }
         } else {
            _context.Error(result.ApiCallDetails.DebugInformation.Replace("{", "{{").Replace("}", "}}"));
         }

         _context.Debug(() => $"Found value: {converted ?? "null"}");
         return converted;
      }

      public void End() {
         throw new NotImplementedException();
      }

      public int GetNextTflBatchId() {

         var result = GetAggregations();

         if (result.ApiCallDetails.HasSuccessfulStatusCode) {
            return DynamicToInt(result.Body["aggregations"]["b"]["value"]) + 1;
         } else {
            _context.Error(result.ApiCallDetails.OriginalException.Message);
            _context.Debug(() => result.ApiCallDetails.DebugInformation);
            return 0;
         }

      }

      public int GetMaxTflKey() {
         var result = GetAggregations();

         if (result.ApiCallDetails.HasSuccessfulStatusCode) {
            return DynamicToInt(result.Body["aggregations"]["k"]["value"]);
         } else {
            _context.Error(result.ApiCallDetails.DebugInformation.Replace("{", "{{").Replace("}", "}}"));
            return 0;
         }
      }

      public void Initialize() {
         throw new NotImplementedException();
      }

      public IEnumerable<IRow> Match(IEnumerable<IRow> rows) {
         throw new NotImplementedException();
      }

      public IEnumerable<IRow> ReadKeys() {
         throw new NotImplementedException();
      }

      public void Start() {
         throw new NotImplementedException();
      }

      public void Write(IEnumerable<IRow> rows) {
         throw new NotImplementedException();
      }

      private DynamicResponse GetAggregations() {

         if (_commonAggregations != null) {
            return _commonAggregations;
         }

         var body = new {
            aggs = new {
               b = new {
                  max = new {
                     field = "tflbatchid"
                  }
               },
               k = new {
                  max = new {
                     field = "tflkey"
                  }
               }
            },
            size = 0
         };

         var aggPath = new EndpointPath(HttpMethod.POST, $"/{_context.Connection.Index}/_search");
         _commonAggregations = _client.Request<DynamicResponse>(in aggPath, PostData.String(JsonConvert.SerializeObject(body)));
         return _commonAggregations;
      }

      private static int DynamicToInt(dynamic dynamicValue) {
         if (dynamicValue == null || !dynamicValue.HasValue) return 0;
         var val = dynamicValue.Value;
         if (val is JsonElement je)
            return je.ValueKind == JsonValueKind.Null ? 0 : (int)je.GetDouble();
         return val == null ? 0 : Convert.ToInt32(val);
      }

      private static long DynamicToLong(dynamic dynamicValue) {
         if (dynamicValue == null || !dynamicValue.HasValue) return 0L;
         var val = dynamicValue.Value;
         if (val is JsonElement je)
            return je.ValueKind == JsonValueKind.Null ? 0L : (long)je.GetDouble();
         return val == null ? 0L : Convert.ToInt64(val);
      }

      private static object DynamicToObject(dynamic dynamicValue) {
         if (dynamicValue == null || !dynamicValue.HasValue) return null;
         var val = dynamicValue.Value;
         if (val is JsonElement je) {
            return je.ValueKind switch {
               JsonValueKind.String => je.GetString(),
               JsonValueKind.Number => je.TryGetInt64(out var l) ? (object)l : je.GetDouble(),
               JsonValueKind.True => true,
               JsonValueKind.False => false,
               JsonValueKind.Null => null,
               _ => je.ToString()
            };
         }
         return val;
      }

      public void Dispose() {
      }

   public Task DeleteAsync(CancellationToken token = default) { Delete(); return Task.CompletedTask; }
   public Task EndAsync(CancellationToken token = default) { End(); return Task.CompletedTask; }

   public async Task<int> GetMaxTflKeyAsync(CancellationToken token = default) {
         var result = await GetAggregationsAsync(token).ConfigureAwait(false);

         if (result.ApiCallDetails.HasSuccessfulStatusCode) {
            return DynamicToInt(result.Body["aggregations"]["k"]["value"]);
         } else {
            _context.Error(result.ApiCallDetails.DebugInformation.Replace("{", "{{").Replace("}", "}}"));
            return 0;
         }
      }

   public async Task<object> GetMaxVersionAsync(CancellationToken token = default) {

         if (string.IsNullOrEmpty(_context.Entity.Version))
            return null;

         var version = _context.Entity.GetVersionField();

         _context.Debug(() => $"Detecting Max Output Version: {_context.Connection.Index}.{_context.TypeName()}.{version.Alias.ToLower()}.");

         var body = new {
            aggs = new {
               version = new {
                  max = new {
                     field = version.Alias.ToLower()
                  }
               }
            },
            size = 0
         };

         var asyncSearchPath = new EndpointPath(HttpMethod.POST, $"/{_context.Connection.Index}/_search");
         var result = await _client.RequestAsync<DynamicResponse>(in asyncSearchPath, PostData.String(JsonConvert.SerializeObject(body)), token).ConfigureAwait(false);
         dynamic converted = null;
         if (result.ApiCallDetails.HasSuccessfulStatusCode) {
            try {
               var dynValue = result.Body["aggregations"]["version"]["value"];
               if (dynValue.HasValue) {
                  if (version.Type.StartsWith("date")) {
                     try {
                        var ms = DynamicToLong(dynValue);
                        converted = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(ms);
                     } catch (Exception e) {
                        _context.Error(e, $"Failed converting {version.Name} value to long (for epoch_millis) conversion to datetime.");
                     }
                  } else {
                     converted = DynamicToObject(dynValue);
                  }
               }
            } catch (Exception ex) {
               _context.Error(ex, ex.Message);
            }
         } else {
            _context.Error(result.ApiCallDetails.DebugInformation.Replace("{", "{{").Replace("}", "}}"));
         }

         _context.Debug(() => $"Found value: {converted ?? "null"}");
         return converted;
      }

   public async Task<int> GetNextTflBatchIdAsync(CancellationToken token = default) {

         var result = await GetAggregationsAsync(token).ConfigureAwait(false);

         if (result.ApiCallDetails.HasSuccessfulStatusCode) {
            return DynamicToInt(result.Body["aggregations"]["b"]["value"]) + 1;
         } else {
            _context.Error(result.ApiCallDetails.OriginalException.Message);
            _context.Debug(() => result.ApiCallDetails.DebugInformation);
            return 0;
         }
      }

   public Task InitializeAsync(CancellationToken token = default) { Initialize(); return Task.CompletedTask; }
   public Task<IEnumerable<IRow>> MatchAsync(IEnumerable<IRow> rows, CancellationToken token = default) { return Task.FromResult(Match(rows)); }
   public Task<IEnumerable<IRow>> ReadKeysAsync(CancellationToken token = default) { return Task.FromResult(ReadKeys()); }
   public Task StartAsync(CancellationToken token = default) { Start(); return Task.CompletedTask; }
   public Task WriteAsync(IEnumerable<IRow> rows, CancellationToken token = default) { Write(rows); return Task.CompletedTask; }

      private async Task<DynamicResponse> GetAggregationsAsync(CancellationToken token = default) {

         if (_commonAggregations != null) {
            return _commonAggregations;
         }

         var body = new {
            aggs = new {
               b = new {
                  max = new {
                     field = "tflbatchid"
                  }
               },
               k = new {
                  max = new {
                     field = "tflkey"
                  }
               }
            },
            size = 0
         };

         var asyncAggPath = new EndpointPath(HttpMethod.POST, $"/{_context.Connection.Index}/_search");
         _commonAggregations = await _client.RequestAsync<DynamicResponse>(in asyncAggPath, PostData.String(JsonConvert.SerializeObject(body)), token).ConfigureAwait(false);
         return _commonAggregations;
      }
   }
}