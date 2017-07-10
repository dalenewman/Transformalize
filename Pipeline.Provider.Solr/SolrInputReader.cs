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
using System.Collections.ObjectModel;
using System.Linq;
using SolrNet;
using SolrNet.Commands.Parameters;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using System.Text.RegularExpressions;

namespace Transformalize.Provider.Solr {
    public class SolrInputReader : IRead {

        private const string PhrasePattern = @"\""(?>[^""]+|\""(?<number>)|\""(?<-number>))*(?(number)(?!))\""";
        private static readonly Regex _phraseRegex = new Regex(PhrasePattern, RegexOptions.Compiled);

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

            AbstractSolrQuery query = SolrQuery.All;
            var filterQueries = new Collection<ISolrQuery>();
            var facetQueries = new Collection<ISolrFacetQuery>();

            if (_context.Entity.Filter.Any()) {
                var queries = new Collection<ISolrQuery>();

                foreach (var filter in _context.Entity.Filter.Where(f => f.Type == "search" && f.Value != "*")) {
                    if (filter.Field == string.Empty) {
                        queries.Add(new SolrQuery(filter.Expression));
                    } else {
                        foreach (var term in Terms(filter.Value)) {
                            queries.Add(new SolrQueryByField(filter.Field, term) { Quoted = false });
                        }
                    }
                }

                query = queries.Any() ? new SolrMultipleCriteriaQuery(queries, "AND") : SolrQuery.All;

                foreach (var filter in _context.Entity.Filter.Where(f => f.Type == "filter")) {
                    if (filter.Field == string.Empty) {
                        filterQueries.Add(new SolrQuery(filter.Expression));
                    } else {
                        if(filter.Value != "*") {
                            foreach (var term in Terms(filter.Value)) {
                                queries.Add(new SolrQueryByField(filter.Field, term) { Quoted = false });
                            }
                        }
                    }
                }

                foreach (var filter in _context.Entity.Filter.Where(f => f.Type == "facet")) {
                    facetQueries.Add(new SolrFacetFieldQuery(filter.Field) {
                        MinCount = filter.Min,
                        Limit = filter.Size
                    });
                    if(filter.Value != "*") {
                        if (filter.Value.IndexOf(',') > 0) {
                            filterQueries.Add(new SolrQueryInList(filter.Field, filter.Value.Split(new[] { ',' })));
                        } else {
                            filterQueries.Add(new SolrQueryByField(filter.Field, filter.Value));
                        }
                    }
                }
            }

            int rows;
            StartOrCursor startOrCursor;
            if (_context.Entity.IsPageRequest()) {
                rows = _context.Entity.PageSize;
                startOrCursor = new StartOrCursor.Start((_context.Entity.Page * _context.Entity.PageSize) - _context.Entity.PageSize);
            } else {
                rows = _context.Entity.ReadSize > 0 ? _context.Entity.ReadSize : _solr.Query(query, new QueryOptions { StartOrCursor = new StartOrCursor.Start(0), Rows = 0 }).NumFound;
                startOrCursor = _context.Entity.ReadSize == 0 ? (StartOrCursor)new StartOrCursor.Start(0) : StartOrCursor.Cursor.Start;
            }

            var sortOrder = new Collection<SortOrder>();
            foreach (var orderBy in _context.Entity.Order) {
                Field field;
                if (_context.Entity.TryGetField(orderBy.Field, out field)) {
                    var name = field.SortField.ToLower();
                    sortOrder.Add(new SortOrder(name, orderBy.Sort == "asc" ? SolrNet.Order.ASC : SolrNet.Order.DESC));
                }
            }
            sortOrder.Add(new SortOrder("score", SolrNet.Order.DESC));

            var result = _solr.Query(
                query,
                new QueryOptions {
                    StartOrCursor = startOrCursor,
                    Rows = rows,
                    Fields = _fieldNames,
                    OrderBy = sortOrder,
                    FilterQueries = filterQueries,
                    Facet = new FacetParameters { Queries = facetQueries, Sort = false }
                }
            );

            foreach (var filter in _context.Entity.Filter.Where(f => f.Type == "facet")) {
                if (result.FacetFields.ContainsKey(filter.Field)) {
                    var facet = result.FacetFields[filter.Field];
                    var map = _context.Process.Maps.First(m => m.Name == filter.Map);
                    foreach (var f in facet) {
                        map.Items.Add(new MapItem { From = $"{f.Key} ({f.Value})", To = f.Key });
                    }
                }
            }

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
                        Fields = _fieldNames,
                        OrderBy = sortOrder,
                        FilterQueries = filterQueries,
                        Facet = new FacetParameters { Queries = facetQueries, Sort = false }
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
                        Fields = _fieldNames,
                        OrderBy = sortOrder,
                        FilterQueries = filterQueries,
                        Facet = new FacetParameters { Queries = facetQueries, Sort = false }
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

        private IEnumerable<string> Terms(string value, string delimiter = " ") {

            var processedValue = value.Trim(delimiter.ToCharArray());

            if (!processedValue.Contains(" "))
                return new[] { processedValue };

            if (processedValue.StartsWith("[") && processedValue.EndsWith("]") && processedValue.Contains(" TO ") ||
                processedValue.StartsWith("{") && processedValue.EndsWith("}") && processedValue.Contains(" TO ")) {
                return new[] { processedValue };
            }

            if (!processedValue.Contains("\""))
                return processedValue.Split(delimiter.ToCharArray());

            var phrases = new List<string>();
            foreach (var match in _phraseRegex.Matches(processedValue)) {
                phrases.Add(match.ToString());
                processedValue = processedValue.Replace(match.ToString(), string.Empty).Trim(delimiter.ToCharArray());
            }

            if (processedValue.Length > 0)
                phrases.AddRange(processedValue.Split(delimiter.ToCharArray()));
            return phrases;
        }
    }
}