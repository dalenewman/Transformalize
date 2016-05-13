#region license
// Transformalize
// A Configurable ETL solution specializing in incremental denormalization.
// Copyright 2013 Dale Newman
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
using Elasticsearch.Net;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Provider.Elastic.Ext;

namespace Pipeline.Provider.Elastic {
    public class ElasticOutputVersionDetector : IVersionDetector {
        private readonly OutputContext _context;
        private readonly IElasticLowLevelClient _client;

        public ElasticOutputVersionDetector(OutputContext context, IElasticLowLevelClient client) {
            _context = context;
            _client = client;
        }

        public object Detect() {

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

            ElasticsearchResponse<DynamicResponse> result = _client.Search<DynamicResponse>(_context.Connection.Index, _context.TypeName(), body);
            dynamic value = null;
            try {
                value = result.Body["aggregations"]["version"]["value"].Value;
            } catch (Exception ex) {
                _context.Error(ex, ex.Message);
            }
            var converted = value ?? null;
            _context.Debug(() => $"Found value: {converted ?? "null"}");
            return converted;
        }
    }
}