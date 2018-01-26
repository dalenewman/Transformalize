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
using System.Text.RegularExpressions;
using Elasticsearch.Net;
using Newtonsoft.Json;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Providers.Elastic {

    public class ElasticReader : IRead {

        private readonly Regex _isQueryString = new Regex(@" OR | AND |\*|\?", RegexOptions.Compiled);

        private readonly IElasticLowLevelClient _client;
        private readonly IConnectionContext _context;
        private readonly Field[] _fields;
        private readonly string[] _fieldNames;
        private readonly IRowFactory _rowFactory;
        private readonly ReadFrom _readFrom;
        private readonly string _typeName;

        public ElasticReader(
            IConnectionContext context,
            Field[] fields,
            IElasticLowLevelClient client,
            IRowFactory rowFactory,
            ReadFrom readFrom
            ) {

            _context = context;
            _fields = fields;
            _fieldNames = fields.Select(f => _readFrom == ReadFrom.Input ? f.Name : f.Alias.ToLower()).ToArray();
            _client = client;
            _rowFactory = rowFactory;
            _readFrom = readFrom;
            _typeName = readFrom == ReadFrom.Input ? context.Entity.Name : context.Entity.Alias.ToLower();

        }

        private string WriteQuery(
            IEnumerable<Field> fields,
            ReadFrom readFrom,
            IContext context,
            int from = 0,
            int size = 10
            ) {

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            using (var writer = new JsonTextWriter(sw)) {
                writer.WriteStartObject();

                writer.WritePropertyName("from");
                writer.WriteValue(from);
                writer.WritePropertyName("size");
                writer.WriteValue(size);

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
                                    if(filter.Expression == string.Empty) {
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

                                        if(filter.Field != string.Empty) {
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
                        Field field;
                        if (context.Entity.TryGetField(orderBy.Field, out field)) {
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

            ElasticsearchResponse<DynamicResponse> response;
            ElasticsearchDynamicValue hits;
            var from = 1;
            var size = 10;
            string body;
            if (_context.Entity.IsPageRequest()) {
                from = (_context.Entity.Page * _context.Entity.Size) - _context.Entity.Size;
                body = WriteQuery(_fields, _readFrom, _context, from, _context.Entity.Size);
            } else {
                body = WriteQuery(_fields, _readFrom, _context, 0, 0);
                response = _client.Search<DynamicResponse>(_context.Connection.Index, _typeName, body);
                if (response.Success) {
                    hits = response.Body["hits"] as ElasticsearchDynamicValue;
                    if (hits != null && hits.HasValue) {
                        var properties = hits.Value as IDictionary<string, object>;
                        if (properties != null && properties.ContainsKey("total")) {
                            size = Convert.ToInt32(properties["total"]);
                            body = WriteQuery(_fields, _readFrom, _context, 0, size > 10000 ? 10000 : size);
                        }
                    }
                }
            }

            _context.Debug(() => body);
            _context.Entity.Query = body;

            // move 10000 to configurable limit
            response = from + size > 10000
                ? _client.Search<DynamicResponse>(_context.Connection.Index, _typeName, body, p => p.Scroll(TimeSpan.FromMinutes(1.0)))
                : _client.Search<DynamicResponse>(_context.Connection.Index, _typeName, body);

            if (!response.Success) {
                LogError(response);
                yield break;
            }

            _context.Entity.Hits = Convert.ToInt32((response.Body["hits"]["total"] as ElasticsearchDynamicValue).Value);
            hits = response.Body["hits"]["hits"] as ElasticsearchDynamicValue;


            if (hits == null || !hits.HasValue) {
                _context.Warn("No hits from elasticsearch");
                yield break;
            }

            var docs = hits.Value as IList<object>;
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
                    row[_fields[i]] = _fields[i].Convert(source[_fieldNames[i]]);
                }
                yield return row;
            }
            count += docs.Count;

            // get this from first search response (maybe), unless you have to aggregate it from all...
            foreach (var filter in _context.Entity.Filter.Where(f => f.Type == "facet" && !string.IsNullOrEmpty(f.Map))) {
                var map = _context.Process.Maps.First(m => m.Name == filter.Map);
                var buckets = response.Body["aggregations"][filter.Key]["buckets"] as ElasticsearchDynamicValue;
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
                _client.ClearScroll<DynamicResponse>(new PostData<object>(new { scroll_id = response.Body["_scroll_id"].Value }));
                yield break;
            }

            var scrolls = new HashSet<string>();

            do {
                var scrollId = response.Body["_scroll_id"].Value;
                scrolls.Add(scrollId);
                response = _client.Scroll<DynamicResponse>(new PostData<object>(new { scroll = "1m", scroll_id = scrollId }));
                if (response.Success) {
                    docs = (IList<object>)response.Body["hits"]["hits"].Value;
                    foreach (var d in docs) {
                        var doc = (IDictionary<string, object>)d;
                        var row = _rowFactory.Create();
                        var source = (IDictionary<string, object>)doc["_source"];
                        for (var i = 0; i < _fields.Length; i++) {
                            row[_fields[i]] = _fields[i].Convert(source[_fieldNames[i]]);
                        }
                        yield return row;
                    }
                    count += docs.Count;
                } else {
                    LogError(response);
                }
            } while (response.Success && count < size);

            _client.ClearScroll<DynamicResponse>(new PostData<object>(new { scroll_id = scrolls.ToArray() }));
        }

        private void LogError(IApiCallDetails response) {
            if (response.ServerError?.Error?.RootCause != null) {
                foreach (var error in response.ServerError.Error.RootCause) {
                    _context.Error(error.Reason.Replace("{", "{{").Replace("}", "}}"));
                }
            } else {
                _context.Error(response.DebugInformation.Replace("{", "{{").Replace("}", "}}"));
            }
        }
    }
}
