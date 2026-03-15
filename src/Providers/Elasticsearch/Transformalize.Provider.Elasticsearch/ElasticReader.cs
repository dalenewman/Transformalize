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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Elastic.Transport;
using Newtonsoft.Json;
using Transformalize.Configuration;
using Transformalize.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Transformalize.Providers.Elasticsearch {

   public class ElasticReader : IRead {

      private readonly Regex _isQueryString = new Regex(@" OR | AND |\*|\?", RegexOptions.Compiled);
      public const int ElasticsearchDefaultSizeLimit = 10000;
      public const int DefaultSize = 100;
      private readonly ITransport _client;
      private readonly IConnectionContext _context;
      private readonly Field[] _fields;
      private readonly string[] _fieldNames;
      private readonly IRowFactory _rowFactory;
      private readonly ReadFrom _readFrom;
      // private readonly string _typeName;
      private readonly Version _version;

      public ElasticReader(
          IConnectionContext context,
          Field[] fields,
          ITransport client,
          IRowFactory rowFactory,
          ReadFrom readFrom
          ) {

         _context = context;
         _fields = fields;
         _fieldNames = fields.Select(f => _readFrom == ReadFrom.Input ? f.Name : f.Alias.ToLower()).ToArray();
         _client = client;
         _rowFactory = rowFactory;
         _readFrom = readFrom;
         // _typeName = readFrom == ReadFrom.Input ? context.Entity.Name : context.Entity.Alias.ToLower();

         _context.Entity.ReadSize = _context.Entity.ReadSize == 0 ? DefaultSize : _context.Entity.ReadSize;
         
         if(_context.Entity.ReadSize > ElasticsearchDefaultSizeLimit) {
            _context.Warn("Elasticsearch's default size limit is 10000.  {0} may be too high.", _context.Entity.ReadSize);
         }

         _version = ElasticVersionParser.ParseVersion(_context);
      }

      private string WriteQuery(
          IEnumerable<Field> fields,
          ReadFrom readFrom,
          IContext context,
          bool scroll,
          int from = 0,
          int size = 10
          ) {

         var sb = new StringBuilder();
         var sw = new StringWriter(sb);

         using (var writer = new JsonTextWriter(sw)) {
            writer.WriteStartObject();

            if (!scroll) {
               writer.WritePropertyName("from");
               writer.WriteValue(from);
            }

            writer.WritePropertyName("size");
            writer.WriteValue(size);
            if (_version.Major >= 6) {  // for now, everything below expects to know total number of hits
               writer.WritePropertyName("track_total_hits");
               writer.WriteValue(true);
            }

            writer.WritePropertyName("_source");
            writer.WriteStartObject();
            writer.WritePropertyName("includes");
            writer.WriteStartArray();
            foreach (var field in fields) {
               writer.WriteValue(readFrom == ReadFrom.Input ? field.Name : field.Alias.ToLower());
            }
            writer.WriteEndArray();
            writer.WriteEndObject();

            if (readFrom == ReadFrom.Input) {
               if (context.Entity.Filter.Any(f => f.Value != "*")) {
                  writer.WritePropertyName("query");
                  writer.WriteStartObject();
                  writer.WritePropertyName("constant_score");
                  writer.WriteStartObject();
                  writer.WritePropertyName("filter");
                  writer.WriteStartObject();
                  writer.WritePropertyName("bool");
                  writer.WriteStartObject();
                  writer.WritePropertyName("must");
                  writer.WriteStartArray();

                  foreach (var filter in context.Entity.Filter.Where(f =>
                      (f.Expression != string.Empty && f.Expression != "*") ||
                      (f.Value != "*" && f.Value != string.Empty))
                  ) {
                     writer.WriteStartObject();

                     switch (filter.Type) {
                        case "facet":
                           if (filter.Value.Contains(",")) {
                              writer.WritePropertyName("terms");
                              writer.WriteStartObject();
                              writer.WritePropertyName(filter.LeftField.Name);

                              writer.WriteStartArray(); // values
                              foreach (var value in filter.Value.Split(',')) {
                                 writer.WriteValue(value);
                              }
                              writer.WriteEndArray(); //values

                              writer.WriteEndObject();

                           } else {
                              writer.WritePropertyName("term");
                              writer.WriteStartObject();
                              writer.WritePropertyName(filter.LeftField.Name);
                              writer.WriteValue(filter.Value);
                              writer.WriteEndObject();
                           }
                           break;
                        case "range":
                           break;
                        default: //search
                           if (filter.Expression == string.Empty) {
                              if (_isQueryString.IsMatch(filter.Value)) {
                                 // query_string query
                                 writer.WritePropertyName("query_string");
                                 writer.WriteStartObject(); // query_string

                                 writer.WritePropertyName("query");
                                 writer.WriteValue(filter.Value);

                                 writer.WritePropertyName("default_field");
                                 writer.WriteValue(filter.LeftField.Name);

                                 writer.WritePropertyName("analyze_wildcard");
                                 writer.WriteValue(true);

                                 writer.WritePropertyName("default_operator");
                                 writer.WriteValue("AND");

                                 writer.WriteEnd(); // query_string
                              } else {
                                 // match query
                                 writer.WritePropertyName("match");
                                 writer.WriteStartObject(); // match

                                 writer.WritePropertyName(filter.LeftField.Name);
                                 writer.WriteStartObject(); // field name

                                 writer.WritePropertyName("query");
                                 writer.WriteValue(filter.Value);

                                 writer.WritePropertyName("operator");
                                 writer.WriteValue(filter.Continuation.ToLower());

                                 writer.WriteEndObject(); // field name
                                 writer.WriteEndObject(); // match
                              }

                           } else {
                              writer.WritePropertyName("query_string");
                              writer.WriteStartObject(); // query_string

                              writer.WritePropertyName("query");
                              writer.WriteValue(filter.Expression);

                              if (filter.Field != string.Empty) {
                                 writer.WritePropertyName("default_field");
                                 writer.WriteValue(filter.LeftField.Name);
                              }

                              writer.WritePropertyName("analyze_wildcard");
                              writer.WriteValue(true);

                              writer.WritePropertyName("default_operator");
                              writer.WriteValue("AND");

                              writer.WriteEnd(); // query_string
                           }

                           break;
                     }
                     writer.WriteEndObject(); //must
                  }

                  writer.WriteEndArray();
                  writer.WriteEndObject(); //bool
                  writer.WriteEndObject(); //filter
                  writer.WriteEndObject(); //constant_score
                  writer.WriteEndObject(); //query
               }
            } else {
               writer.WritePropertyName("query");
               writer.WriteStartObject();
               writer.WritePropertyName("constant_score");
               writer.WriteStartObject();
               writer.WritePropertyName("filter");
               writer.WriteStartObject();
               writer.WritePropertyName("term");
               writer.WriteStartObject();
               writer.WritePropertyName("tfldeleted");
               writer.WriteValue(false);
               writer.WriteEndObject();
               writer.WriteEndObject();
               writer.WriteEndObject();
               writer.WriteEndObject();

            }

            if (context.Entity.Order.Any()) {
               writer.WritePropertyName("sort");
               writer.WriteStartArray();

               foreach (var orderBy in context.Entity.Order) {
                  if (context.Entity.TryGetField(orderBy.Field, out var field)) {
                     var name = _readFrom == ReadFrom.Input ? field.SortField.ToLower() : field.Alias.ToLower();
                     writer.WriteStartObject();
                     writer.WritePropertyName(name);
                     writer.WriteStartObject();
                     writer.WritePropertyName("order");
                     writer.WriteValue(orderBy.Sort);
                     writer.WriteEndObject();
                     writer.WriteEndObject();
                  }
               }

               writer.WriteEndArray();

            }

            if (_readFrom == ReadFrom.Input && context.Entity.Filter.Any(f => f.Type == "facet")) {

               writer.WritePropertyName("aggs");
               writer.WriteStartObject();
               foreach (var filter in context.Entity.Filter.Where(f => f.Type == "facet")) {

                  writer.WritePropertyName(filter.Key);
                  writer.WriteStartObject();

                  writer.WritePropertyName("terms");
                  writer.WriteStartObject();
                  writer.WritePropertyName("field");
                  writer.WriteValue(filter.LeftField.Name);
                  writer.WritePropertyName("size");
                  writer.WriteValue(filter.Size);
                  writer.WritePropertyName("min_doc_count");
                  writer.WriteValue(filter.Min);

                  writer.WritePropertyName("order");
                  writer.WriteStartObject();
                  writer.WritePropertyName(filter.OrderBy);
                  writer.WriteValue(filter.Order);
                  writer.WriteEndObject(); //order


                  writer.WriteEndObject(); // terms

                  writer.WriteEndObject(); // the field name + _filter
               }
               writer.WriteEndObject(); //aggs
            }

            writer.WriteEndObject();
            writer.Flush();
            return sb.ToString();
         }

      }

      public IEnumerable<IRow> Read() {

         DynamicResponse response;
         dynamic hits;

         var from = 0;
         var size = 10;
         string body;
         bool warned = false;

         var scroll = !_context.Entity.IsPageRequest();

         if (!scroll) {
            from = (_context.Entity.Page * _context.Entity.Size) - _context.Entity.Size;
            body = WriteQuery(_fields, _readFrom, _context, scroll:false, from: from, size: _context.Entity.Size);
         } else {
            body = WriteQuery(_fields, _readFrom, _context, scroll:false, from: 0, size: 0);
            var countPath = new EndpointPath(HttpMethod.POST, $"/{_context.Connection.Index}/_search");
            response = _client.Request<DynamicResponse>(ref countPath, PostData.String(body));
            if (response.ApiCallDetails.HasSuccessfulStatusCode) {
               hits = response.Body["hits"];
               if (hits != null && hits.HasValue) {
                  var total = hits["total"];

                  try {
                     if (_version.Major >= 7) {  // version 7 changed total to an object with "value" and "relation" properties
                        size = DynamicToInt(total["value"]);
                     } else {
                        size = DynamicToInt(total);
                     }
                  } catch (Exception ex) {
                     warned = true;
                     _context.Debug(() => total);
                     _context.Warn($"Could not get total number of matching documents from the elasticsearch response.  Are you sure you using version {_version}?");
                     _context.Error(ex, ex.Message);
                  }
               }
            }
            body = WriteQuery(_fields, _readFrom, _context, scroll:true, from: 0, size: size > ElasticsearchDefaultSizeLimit ? DefaultSize : size);
         }

         _context.Debug(() => body);
         _context.Entity.Query = body;

         if (scroll) {
            var scrollSearchPath = new EndpointPath(HttpMethod.POST, $"/{_context.Connection.Index}/_search?scroll={_context.Connection.Scroll}");
            response = _client.Request<DynamicResponse>(ref scrollSearchPath, PostData.String(body));
         } else {
            var searchPath = new EndpointPath(HttpMethod.POST, $"/{_context.Connection.Index}/_search");
            response = _client.Request<DynamicResponse>(ref searchPath, PostData.String(body));
         }

         if (!response.ApiCallDetails.HasSuccessfulStatusCode) {
            LogError(response);
            yield break;
         }

         try {
            if (_version.Major >= 7) {  // version 7 changed total to an object with "value" and "relation" properties
               _context.Entity.Hits = DynamicToInt(response.Body["hits"]["total"]["value"]);
            } else {
               _context.Entity.Hits = DynamicToInt(response.Body["hits"]["total"]);
            }
         } catch (Exception ex) {
            if (!warned) {
               _context.Debug(() => response.Body["hits"]);
               _context.Warn($"Could not get total number of matching documents from the elasticsearch response.  Are you sure you using version {_version}?");
               _context.Error(ex.Message);
            }
         }

         hits = response.Body["hits"]["hits"];

         if (hits == null || !hits.HasValue) {
            _context.Warn("No hits from elasticsearch");
            yield break;
         }

         var docs = ExtractDocs((object)hits);
         if (docs == null) {
            _context.Error("No documents returned from elasticsearch!");
            yield break;
         }

         // if any of the fields do not exist, yield break
         if (docs.Count > 0) {
            var doc = docs.First() as IDictionary<string, object>;
            var source = doc?["_source"] as IDictionary<string, object>;
            if (source == null) {
               _context.Error("Missing _source from elasticsearch response!");
               yield break;
            }

            for (var i = 0; i < _fields.Length; i++) {
               if (source.ContainsKey(_fieldNames[i]))
                  continue;

               _context.Error($"Field {_fieldNames[i]} does not exist!");
               yield break;
            }
         }

         var count = 0;
         foreach (var d in docs) {
            var doc = (IDictionary<string, object>)d;
            var row = _rowFactory.Create();
            var source = (IDictionary<string, object>)doc["_source"];
            for (var i = 0; i < _fields.Length; i++) {
               row[_fields[i]] = _fields[i].Convert(UnwrapJsonElement(source[_fieldNames[i]]));
            }
            yield return row;
         }
         count += docs.Count;

         // get this from first search response (maybe), unless you have to aggregate it from all...
         foreach (var filter in _context.Entity.Filter.Where(f => f.Type == "facet" && !string.IsNullOrEmpty(f.Map))) {
            var map = _context.Process.Maps.First(m => m.Name == filter.Map);
            var buckets = response.Body["aggregations"][filter.Key]["buckets"];
            if (buckets == null || !buckets.HasValue)
               continue;

            var items = buckets.Value as IEnumerable<object>;

            if (items == null)
               continue;

            foreach (var item in items.OfType<IDictionary<string, object>>()) {
               map.Items.Add(new MapItem { From = $"{item["key"]} ({item["doc_count"]})", To = item["key"] });
            }

         }

         if (!response.Body.ContainsKey("_scroll_id"))
            yield break;

         if (size == count) {
            var clearScrollPath = new EndpointPath(HttpMethod.DELETE, "/_search/scroll");
            _client.Request<DynamicResponse>(ref clearScrollPath, PostData.String(JsonConvert.SerializeObject(new { scroll_id = response.Body["_scroll_id"].Value })));
            yield break;
         }

         var scrolls = new HashSet<string>();

         do {
            var scrollId = response.Body["_scroll_id"].Value;
            scrolls.Add(scrollId.ToString());
            var doScrollPath = new EndpointPath(HttpMethod.POST, "/_search/scroll");
            response = _client.Request<DynamicResponse>(ref doScrollPath, PostData.String(JsonConvert.SerializeObject(new { scroll = _context.Connection.Scroll, scroll_id = scrollId })));
            if (response.ApiCallDetails.HasSuccessfulStatusCode) {
               docs = ExtractDocs((object)response.Body["hits"]["hits"]);
               if (docs != null) {
                  foreach (var d in docs) {
                     var doc = (IDictionary<string, object>)d;
                     var row = _rowFactory.Create();
                     var source = (IDictionary<string, object>)doc["_source"];
                     for (var i = 0; i < _fields.Length; i++) {
                        row[_fields[i]] = _fields[i].Convert(UnwrapJsonElement(source[_fieldNames[i]]));
                     }
                     yield return row;
                  }
                  count += docs.Count;
               }
            } else {
               LogError(response);
            }
         } while (response.ApiCallDetails.HasSuccessfulStatusCode && count < size);

         var finalClearScrollPath = new EndpointPath(HttpMethod.DELETE, "/_search/scroll");
         _client.Request<DynamicResponse>(ref finalClearScrollPath, PostData.String(JsonConvert.SerializeObject(new { scroll_id = scrolls.ToArray() })));
      }

      private static bool Scroll(int from, int size) {
         return from + size > ElasticsearchDefaultSizeLimit;
      }

      private static IList<object> ExtractDocs(object hitsObj) {
         if (hitsObj is not DynamicValue hitsValue || !hitsValue.HasValue) return null;
         var val = hitsValue.Value;
         if (val is IList<object> list) return list;
         if (val is JsonElement je && je.ValueKind == JsonValueKind.Array)
            return DynamicValue.ConsumeJsonElement(typeof(object), je) as IList<object>;
         return null;
      }

      private static object UnwrapJsonElement(object value) {
         if (value is JsonElement je) {
            return je.ValueKind switch {
               JsonValueKind.String => je.GetString(),
               JsonValueKind.Number => je.TryGetInt64(out var l) ? (object)l : je.GetDouble(),
               JsonValueKind.True => true,
               JsonValueKind.False => false,
               JsonValueKind.Null => null,
               _ => je.ToString()
            };
         }
         return value;
      }

      private static int DynamicToInt(dynamic dynamicValue) {
         if (dynamicValue == null || !dynamicValue.HasValue) return 0;
         var val = dynamicValue.Value;
         if (val is JsonElement je)
            return je.ValueKind == JsonValueKind.Null ? 0 : (int)je.GetDouble();
         return val == null ? 0 : Convert.ToInt32(val);
      }

      private void LogError(DynamicResponse response) {
         _context.Error(response.ApiCallDetails.DebugInformation.Replace("{", "{{").Replace("}", "}}"));
      }

      public async Task<IEnumerable<IRow>> ReadAsync(CancellationToken token = default) {

         var results = new List<IRow>();

         DynamicResponse response;
         dynamic hits;

         var from = 0;
         var size = 10;
         string body;
         bool warned = false;

         var scroll = !_context.Entity.IsPageRequest();

         if (!scroll) {
            from = (_context.Entity.Page * _context.Entity.Size) - _context.Entity.Size;
            body = WriteQuery(_fields, _readFrom, _context, scroll: false, from: from, size: _context.Entity.Size);
         } else {
            body = WriteQuery(_fields, _readFrom, _context, scroll: false, from: 0, size: 0);
            var asyncCountPath = new EndpointPath(HttpMethod.POST, $"/{_context.Connection.Index}/_search");
            response = await _client.RequestAsync<DynamicResponse>(ref asyncCountPath, PostData.String(body), token).ConfigureAwait(false);
            if (response.ApiCallDetails.HasSuccessfulStatusCode) {
               hits = response.Body["hits"];
               if (hits != null && hits.HasValue) {
                  var total = hits["total"];

                  try {
                     if (_version.Major >= 7) {
                        size = DynamicToInt(total["value"]);
                     } else {
                        size = DynamicToInt(total);
                     }
                  } catch (Exception ex) {
                     warned = true;
                     _context.Debug(() => total);
                     _context.Warn($"Could not get total number of matching documents from the elasticsearch response.  Are you sure you using version {_version}?");
                     _context.Error(ex, ex.Message);
                  }
               }
            }
            body = WriteQuery(_fields, _readFrom, _context, scroll: true, from: 0, size: size > ElasticsearchDefaultSizeLimit ? DefaultSize : size);
         }

         _context.Debug(() => body);
         _context.Entity.Query = body;

         if (scroll) {
            var asyncScrollSearchPath = new EndpointPath(HttpMethod.POST, $"/{_context.Connection.Index}/_search?scroll={_context.Connection.Scroll}");
            response = await _client.RequestAsync<DynamicResponse>(ref asyncScrollSearchPath, PostData.String(body), token).ConfigureAwait(false);
         } else {
            var asyncSearchPath = new EndpointPath(HttpMethod.POST, $"/{_context.Connection.Index}/_search");
            response = await _client.RequestAsync<DynamicResponse>(ref asyncSearchPath, PostData.String(body), token).ConfigureAwait(false);
         }

         if (!response.ApiCallDetails.HasSuccessfulStatusCode) {
            LogError(response);
            return results;
         }

         try {
            if (_version.Major >= 7) {
               _context.Entity.Hits = DynamicToInt(response.Body["hits"]["total"]["value"]);
            } else {
               _context.Entity.Hits = DynamicToInt(response.Body["hits"]["total"]);
            }
         } catch (Exception ex) {
            if (!warned) {
               _context.Debug(() => response.Body["hits"]);
               _context.Warn($"Could not get total number of matching documents from the elasticsearch response.  Are you sure you using version {_version}?");
               _context.Error(ex.Message);
            }
         }

         hits = response.Body["hits"]["hits"];

         if (hits == null || !hits.HasValue) {
            _context.Warn("No hits from elasticsearch");
            return results;
         }

         var docs = ExtractDocs((object)hits);
         if (docs == null) {
            _context.Error("No documents returned from elasticsearch!");
            return results;
         }

         // if any of the fields do not exist, return empty
         if (docs.Count > 0) {
            var doc = docs.First() as IDictionary<string, object>;
            var source = doc?["_source"] as IDictionary<string, object>;
            if (source == null) {
               _context.Error("Missing _source from elasticsearch response!");
               return results;
            }

            for (var i = 0; i < _fields.Length; i++) {
               if (source.ContainsKey(_fieldNames[i]))
                  continue;

               _context.Error($"Field {_fieldNames[i]} does not exist!");
               return results;
            }
         }

         var count = 0;
         foreach (var d in docs) {
            var doc = (IDictionary<string, object>)d;
            var row = _rowFactory.Create();
            var source = (IDictionary<string, object>)doc["_source"];
            for (var i = 0; i < _fields.Length; i++) {
               row[_fields[i]] = _fields[i].Convert(UnwrapJsonElement(source[_fieldNames[i]]));
            }
            results.Add(row);
         }
         count += docs.Count;

         // get this from first search response (maybe), unless you have to aggregate it from all...
         foreach (var filter in _context.Entity.Filter.Where(f => f.Type == "facet" && !string.IsNullOrEmpty(f.Map))) {
            var map = _context.Process.Maps.First(m => m.Name == filter.Map);
            var buckets = response.Body["aggregations"][filter.Key]["buckets"];
            if (buckets == null || !buckets.HasValue)
               continue;

            var items = buckets.Value as IEnumerable<object>;

            if (items == null)
               continue;

            foreach (var item in items.OfType<IDictionary<string, object>>()) {
               map.Items.Add(new MapItem { From = $"{item["key"]} ({item["doc_count"]})", To = item["key"] });
            }
         }

         if (!response.Body.ContainsKey("_scroll_id"))
            return results;

         if (size == count) {
            var asyncClearPath = new EndpointPath(HttpMethod.DELETE, "/_search/scroll");
            await _client.RequestAsync<DynamicResponse>(ref asyncClearPath, PostData.String(JsonConvert.SerializeObject(new { scroll_id = response.Body["_scroll_id"].Value })), token).ConfigureAwait(false);
            return results;
         }

         var scrolls = new HashSet<string>();

         do {
            var scrollId = response.Body["_scroll_id"].Value;
            scrolls.Add(scrollId.ToString());
            var asyncDoScrollPath = new EndpointPath(HttpMethod.POST, "/_search/scroll");
            response = await _client.RequestAsync<DynamicResponse>(ref asyncDoScrollPath, PostData.String(JsonConvert.SerializeObject(new { scroll = _context.Connection.Scroll, scroll_id = scrollId })), token).ConfigureAwait(false);
            if (response.ApiCallDetails.HasSuccessfulStatusCode) {
               docs = ExtractDocs((object)response.Body["hits"]["hits"]);
               if (docs != null) {
                  foreach (var d in docs) {
                     var doc = (IDictionary<string, object>)d;
                     var row = _rowFactory.Create();
                     var source = (IDictionary<string, object>)doc["_source"];
                     for (var i = 0; i < _fields.Length; i++) {
                        row[_fields[i]] = _fields[i].Convert(UnwrapJsonElement(source[_fieldNames[i]]));
                     }
                     results.Add(row);
                  }
                  count += docs.Count;
               }
            } else {
               LogError(response);
            }
         } while (response.ApiCallDetails.HasSuccessfulStatusCode && count < size);

         var asyncFinalClearScrollPath = new EndpointPath(HttpMethod.DELETE, "/_search/scroll");
         await _client.RequestAsync<DynamicResponse>(ref asyncFinalClearScrollPath, PostData.String(JsonConvert.SerializeObject(new { scroll_id = scrolls.ToArray() })), token).ConfigureAwait(false);

         return results;
      }
   }
}
