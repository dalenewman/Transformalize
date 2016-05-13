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
using Nest;
using Pipeline.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Pipeline.Provider.Elastic {

    public class ElasticReader : IRead {

        readonly IConnectionContext _context;
        readonly Configuration.Field[] _fields;
        readonly Field[] _nestFields;
        readonly string[] _fieldNames;
        readonly IElasticClient _client;
        readonly IRowFactory _rowFactory;
        readonly ReadFrom _readFrom;
        readonly string _typeName;
        readonly QueryContainerDescriptor<dynamic> _query;

        public ElasticReader(
            IConnectionContext context,
            Configuration.Field[] fields,
            IElasticClient client,
            IRowFactory rowFactory,
            ReadFrom readFrom
        ) {

            //TODO: Refactor, got a lot going on here

            _context = context;
            _fields = fields;
            _nestFields = _fields.Select(f => new Field() { Name = f.Name }).ToArray();
            _fieldNames = fields.Select(f => f.Name).ToArray();
            _client = client;
            _rowFactory = rowFactory;
            _readFrom = readFrom;
            _typeName = readFrom == ReadFrom.Input ? context.Entity.Name : context.Entity.Alias.ToLower();

            _query = new QueryContainerDescriptor<dynamic>();

            if (readFrom == ReadFrom.Input) {
                if (context.Entity.Filter.Any()) { 
                    foreach (var filter in context.Entity.Filter) {
                        _query.Term(filter.LeftField.Name, filter.Value);
                    }
                } else {
                    _query.MatchAll();
                }
            } else {
                _query.Term("tfldeleted", false);
            }

            if (context.Entity.Order.Any()) {
                foreach (var orderBy in context.Entity.Order) {
                    Configuration.Field field;
                    if (context.Entity.TryGetField(orderBy.Field, out field)) {
                        var name = _readFrom == ReadFrom.Input ? field.Name : field.Alias.ToLower();
                        SortFieldDescriptor<dynamic> sortField = new SortFieldDescriptor<dynamic>();
                        sortField.Field(name);
                        if (orderBy.Sort == "asc") {
                            sortField.Ascending();
                        } else {
                            sortField.Descending();
                        }
                    }
                }
            }
        }

        public IEnumerable<IRow> Read() {

            //TODO: Respect filters, and optionally order by

            ISearchResponse<dynamic> scanResults;
            ISearchResponse<dynamic> results;

            // note: sort is not implemented
            scanResults = _client.Search<dynamic>(s => s
                    .Index(_context.Connection.Index)
                    .Type(_typeName)
                    .From(0)
                    .Size(_context.Entity.ReadSize)
                    .Query(q=>_query)
                    .Fields(f=>f.Fields(_fieldNames))
                    .SearchType(Elasticsearch.Net.SearchType.Scan)
                    .Scroll("2s")
            );

            results = _client.Scroll<dynamic>("2s", scanResults.ScrollId);

            while (results.Fields.Any()) {
                var localResults = results;
                foreach (var hit in localResults.Hits) {
                    var row = _rowFactory.Create();
                    for (int i = 0; i < _fields.Length; i++) {
                        row[_fields[i]] = hit.Fields.Value<object[]>(_nestFields[i]);
                    }
                    _context.Increment();
                    yield return row;
                }
                results = _client.Scroll<dynamic>("2s", localResults.ScrollId);
            }
        }
    }
}
