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
using Elasticsearch.Net;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Provider.Elastic.Ext;

namespace Transformalize.Provider.Elastic {
    public class ElasticOutputProvider : IOutputProvider {

        private readonly OutputContext _context;
        private readonly IElasticLowLevelClient _client;
        private ElasticsearchResponse<DynamicResponse> _commonAggregations;

        public ElasticOutputProvider(OutputContext context, IElasticLowLevelClient client) {
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

            var result = _client.Search<DynamicResponse>(_context.Connection.Index, _context.TypeName(), new PostData<object>(body));
            dynamic value = null;
            if (result.Success) {
                try {
                    value = result.Body["aggregations"]["version"]["value"].Value;
                } catch (Exception ex) {
                    _context.Error(ex, ex.Message);
                }
            } else {
                _context.Error(result.ServerError.ToString());
                _context.Debug(() => result.DebugInformation);
            }
            var converted = value ?? null;
            _context.Debug(() => $"Found value: {converted ?? "null"}");
            return converted;
        }

        public void End() {
            throw new NotImplementedException();
        }

        public int GetNextTflBatchId() {

            var result = GetAggregations();

            if (result.Success) {
                var batchId = result.Body["aggregations"]["b"]["value"].Value;
                return (batchId == null ? 0 : (int)batchId) + 1;
            } else {
                _context.Error(result.ServerError.ToString());
                _context.Debug(() => result.DebugInformation);
                return 0;
            }

        }

        public int GetMaxTflKey() {
            var result = GetAggregations();

            if (result.Success) {
                var key = result.Body["aggregations"]["k"]["value"].Value;
                return (key == null ? 0 : (int)key);
            } else {
                _context.Error(result.ServerError.ToString());
                _context.Debug(() => result.DebugInformation);
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

        private ElasticsearchResponse<DynamicResponse> GetAggregations() {

            if(_commonAggregations != null) {
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

            _commonAggregations = _client.Search<DynamicResponse>(_context.Connection.Index, _context.TypeName(), new PostData<object>(body));
            return _commonAggregations;
        }

        public void Dispose() {
        }
    }
}