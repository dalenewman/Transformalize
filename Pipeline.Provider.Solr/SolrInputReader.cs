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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace Pipeline.Provider.Solr {

    public class SolrInputReader : IRead {

        private readonly ISolrReadOnlyOperations<Dictionary<string, object>> _solr;
        private readonly InputContext _context;
        private int _localCount;
        private readonly Collection<string> _fieldNames;
        private readonly Field[] _fields;
        private readonly IRowFactory _rowFactory;

        public SolrInputReader(
            ISolrReadOnlyOperations<Dictionary<string, object>> solr, 
            InputContext context, 
            Field[] fields,
            IRowFactory rowFactory
        ) {
            _solr = solr;
            _context = context;
            _fields = fields;
            _rowFactory = rowFactory;
            _localCount = 0;
            _fieldNames = new Collection<string>(fields.Select(f => f.Name).ToList());
        }

        public IEnumerable<IRow> Read() {

            var query = _context.Entity.Filter.Any() ? new SolrMultipleCriteriaQuery(_context.Entity.Filter.Select(f => new SolrQuery(f.Expression)), "AND") : SolrQuery.All;

            int rows;
            StartOrCursor startOrCursor;
            if (_context.Entity.IsPageRequest()) {
                rows = _context.Entity.PageSize;
                startOrCursor = new StartOrCursor.Start((_context.Entity.Page * _context.Entity.PageSize) - _context.Entity.PageSize);
            } else {
                rows = _context.Entity.ReadSize > 0 ? _context.Entity.ReadSize : _solr.Query(query, new QueryOptions { StartOrCursor = new StartOrCursor.Start(0), Rows = 0 }).NumFound;
                startOrCursor = _context.Entity.ReadSize == 0 ? (StartOrCursor)new StartOrCursor.Start(0) : StartOrCursor.Cursor.Start;
            }

            var result = _solr.Query(
                query,
                new QueryOptions {
                    StartOrCursor = startOrCursor,
                    Rows = rows,
                    Fields = _fieldNames
                }
            );

            if (result.NumFound <= 0)
                yield break;

            _context.Entity.Hits = result.NumFound;

            foreach (var row in result.Select(x => DocToRow(_rowFactory.Create(), _fields, x))) {
                _context.Increment();
                ++_localCount;
                yield return row;
            }

            // using cursor, solr 4.7+ (un-tested)
            while (result.NextCursorMark != null) {
                result = _solr.Query(
                    query,
                    new QueryOptions {
                        StartOrCursor = result.NextCursorMark,
                        Rows = _context.Entity.ReadSize,
                        Fields = _fieldNames
                    }
                );

                foreach (var row in result.Select(r => DocToRow(_rowFactory.Create(), _fields, r))) {
                    _context.Increment();
                    ++_localCount;
                    yield return row;
                }
            }

            // traditional paging
            if (_context.Entity.ReadSize == 0 || _localCount >= result.NumFound)
                yield break;

            var pages = result.NumFound / _context.Entity.ReadSize;
            for (var page = 1; page <= pages; page++) {
                result = _solr.Query(
                    query,
                    new QueryOptions {
                        StartOrCursor = new StartOrCursor.Start(page * _context.Entity.ReadSize),
                        Rows = _context.Entity.ReadSize,
                        Fields = _fieldNames
                    }
                    );

                foreach (var row in result.Select(r => DocToRow(_rowFactory.Create(), _fields, r))) {
                    ++_localCount;
                    _context.Increment();
                    yield return row;
                }
            }
        }

        private static IRow DocToRow(IRow row, Field[] fields, IReadOnlyDictionary<string, object> doc) {
            foreach (var field in fields) {
                row[field] = doc[field.Name];
            }
            return row;
        }
    }
}