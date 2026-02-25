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
using Newtonsoft.Json;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.Elasticsearch {

    public class ElasticPartialUpdater : IDelete, IWrite {

        readonly IElasticLowLevelClient _client;
        readonly Configuration.Field[] _fields;
        readonly OutputContext _context;
        // private readonly string _type;
        private readonly string _index;

        public ElasticPartialUpdater(OutputContext context, Configuration.Field[] fields, IElasticLowLevelClient client) {
            _context = context;
            _fields = fields;
            _client = client;
            _index = context.Connection.Index;
            // _type = context.Entity.Alias.ToLower();
        }

        public void Delete(IEnumerable<IRow> rows) {
            // Could probably do bulk updates with partition and bulk operation
            foreach (var row in rows) {
                var id = string.Concat(_context.OutputFields.Where(f => f.PrimaryKey).Select(f => row[f]));
                _client.Update<VoidResponse>(_index, id, PostData.String(JsonConvert.SerializeObject(row.ToExpandoObject(_fields))));
            }
        }

        public void Write(IEnumerable<IRow> rows) {
            foreach (var row in rows) {
                var id = string.Concat(_context.OutputFields.Where(f => f.PrimaryKey).Select(f => row[f]));
                _client.Update<VoidResponse>(_index, id, PostData.String(JsonConvert.SerializeObject(row.ToExpandoObject(_fields))));
            }
        }
    }
}
