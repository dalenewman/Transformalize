#region license
// Transformalize
// Copyright 2013 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using Elasticsearch.Net;
using Pipeline.Context;
using Pipeline.Contracts;

namespace Pipeline.Provider.Elastic {
    public class ElasticInitializer : IInitializer {
        private readonly OutputContext _context;
        private readonly IElasticLowLevelClient _client;

        public ElasticInitializer(OutputContext context, IElasticLowLevelClient client) {
            _context = context;
            _client = client;
        }

        public ActionResponse Execute() {
            if (_client.IndicesExists<DynamicResponse>(_context.Connection.Index).HttpStatusCode == 200) {
                _client.IndicesDelete<VoidResponse>(_context.Connection.Index);
            }
            var elasticResponse = _client.IndicesCreate<DynamicResponse>(_context.Connection.Index, "{ \"settings\":{}}");
            return new ActionResponse {
                Code = elasticResponse.HttpStatusCode ?? 500,
                Content = elasticResponse.ServerError == null
                    ? string.Empty
                    : elasticResponse.ServerError.Error.Reason ?? string.Empty
            };
        }
    }
}