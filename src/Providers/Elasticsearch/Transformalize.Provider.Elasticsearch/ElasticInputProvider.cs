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
using Elastic.Transport;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Providers.Elasticsearch.Ext;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Transformalize.Providers.Elasticsearch {
    public class ElasticInputProvider : IInputProvider {

        readonly InputContext _context;
        readonly ITransport _client;

        public ElasticInputProvider(InputContext context, ITransport client) {
            _context = context;
            _client = client;
        }

        public object GetMaxVersion() {
            
            //TODO: Implement Filter

            if (string.IsNullOrEmpty(_context.Entity.Version))
                return null;

            var version = _context.Entity.GetVersionField();

            _context.Debug(()=>$"Detecting Max Input Version: {_context.Connection.Index}.{_context.TypeName()}.{version.Alias.ToLower()}.");

            var body = new {
                aggs = new {
                    version = new {
                        max = new {
                            field = version.Name.ToLower()
                        }
                    }
                },
                size = 0
            };
            var json = JsonConvert.SerializeObject(body);
            var searchPath = new EndpointPath(HttpMethod.POST, $"/{_context.Connection.Index}/_search");
            var result = _client.Request<DynamicResponse>(in searchPath, PostData.String(json));

            var value = version.Convert(result.Body["aggregations"]["version"]["value"].Value);
            _context.Debug(()=>$"Found value: {value}");
            return value;
        }

        public Schema GetSchema(Entity entity = null) {
            throw new NotImplementedException();
        }

        public IEnumerable<IRow> Read() {
            throw new NotImplementedException();
        }

    public async Task<object> GetMaxVersionAsync(CancellationToken token = default) {

            if (string.IsNullOrEmpty(_context.Entity.Version))
                return null;

            var version = _context.Entity.GetVersionField();

            _context.Debug(() => $"Detecting Max Input Version: {_context.Connection.Index}.{_context.TypeName()}.{version.Alias.ToLower()}.");

            var body = new {
                aggs = new {
                    version = new {
                        max = new {
                            field = version.Name.ToLower()
                        }
                    }
                },
                size = 0
            };
            var json = JsonConvert.SerializeObject(body);
            var asyncSearchPath = new EndpointPath(HttpMethod.POST, $"/{_context.Connection.Index}/_search");
            var result = await _client.RequestAsync<DynamicResponse>(in asyncSearchPath, PostData.String(json), token).ConfigureAwait(false);

            var value = version.Convert(result.Body["aggregations"]["version"]["value"].Value);
            _context.Debug(() => $"Found value: {value}");
            return value;
        }

    public Task<Schema> GetSchemaAsync(Entity entity = null, CancellationToken token = default) { return Task.FromResult(GetSchema(entity)); }
    public Task<IEnumerable<IRow>> ReadAsync(CancellationToken token = default) { return Task.FromResult(Read()); }
    }
}
