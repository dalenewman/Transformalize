#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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

using Elasticsearch.Net;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Provider.Elastic.Ext;

namespace Transformalize.Provider.Elastic {
    public class ElasticInputVersionDetector : IInputVersionDetector {
        readonly InputContext _context;
        readonly IElasticLowLevelClient _client;

        public ElasticInputVersionDetector(InputContext context, IElasticLowLevelClient client) {
            _context = context;
            _client = client;
        }

        public object Detect() {
            
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
            var result = _client.Search<DynamicResponse>(_context.Connection.Index, _context.TypeName(), body);
            var value = version.Convert(result.Body["aggregations"]["version"]["value"].Value);
            _context.Debug(()=>$"Found value: {value}");
            return value;
        }
    }
}
