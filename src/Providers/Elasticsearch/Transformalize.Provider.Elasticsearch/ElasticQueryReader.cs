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
using Elasticsearch.Net;
using Newtonsoft.Json.Linq;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.Elasticsearch {

    public class ElasticQueryReader : IRead {

        readonly IElasticLowLevelClient _client;
        private readonly IRowFactory _rowFactory;
        readonly InputContext _context;
        private readonly Dictionary<string, Field> _fields;
        private readonly HashSet<string> _missing = new HashSet<string>();

        public ElasticQueryReader(
            InputContext context,
            IElasticLowLevelClient client,
            IRowFactory rowFactory
        ) {
            _context = context;
            _client = client;
            _rowFactory = rowFactory;
            _fields = context.InputFields.ToDictionary(k => k.Name, v => v);
        }

        public IEnumerable<IRow> Read() {
            _context.Debug(() => _context.Entity.Query);
            var response = _client.Search<DynamicResponse>(PostData.String(_context.Entity.Query));

            if (response.Success) {
                if (response.Body != null && response.Body["aggregations"].HasValue) {
                    return Flatten("aggregations", response.Body["aggregations"].Value);
                }
                _context.Warn("An elastic query should return aggregations, but yours does not.");
            } else {
                _context.Error(response.DebugInformation.Replace("{", "{{").Replace("}", "}}"));
            }
            return new IRow[0];
        }

        private IEnumerable<IRow> Flatten(string key, object obj, LinkedList<IRow> results = null) {

            IField field = _fields.ContainsKey(key) ? _fields[key] : null;

            // in the beginning, create an empty result
            if (results == null) {
                results = new LinkedList<IRow>();
            }

            if (obj == null)
                return results;

            var dict = obj as IDictionary<string, object>;
            if (dict == null) {
                var list = obj as IList<object>;
                if (list != null) {
                    foreach (var item in list) {
                        results.AddLast(_rowFactory.Create());
                        Flatten(key, item, results);
                    }
                }
            } else {
                if (dict.Count == 1 && dict.ContainsKey("value")) {

                    if (field == null) {
                        _missing.Add(key);
                    } else {
                        // set a field value
                        var value = dict["value"];
                        if (results.Last.Value[field] == null) {
                            results.Last.Value[field] = value;
                        } else {
                            // this must be a total
                            var total = _rowFactory.Create();
                            total[field] = value;
                            results.AddLast(total);
                        }

                    }
                } else {
                    if (dict.ContainsKey("buckets")) {
                        Flatten(key, dict["buckets"], results);
                    } else {
                        if (dict.ContainsKey("key") && dict.ContainsKey("doc_count")) {
                            foreach (var pair in dict) {
                                if (pair.Key == "key") {
                                    if (field == null) {
                                        _missing.Add(key);
                                    } else {
                                        // set a key field value
                                        results.Last()[field] = pair.Value;
                                    }
                                } else {
                                    if (pair.Key != "doc_count") {
                                        Flatten(pair.Key, pair.Value, results);
                                    }
                                }
                            }
                        } else {
                            foreach (var pair in dict) {
                                Flatten(pair.Key, pair.Value, results);
                            }
                        }
                    }
                }
            }

            if (!_missing.Any())
                return results;

            foreach (var missing in _missing) {
                _context.Warn($"The query returns field {missing}, but you do not have that field defined in {_context.Entity.Alias}.");
            }
            return results;
        }

    }
}
