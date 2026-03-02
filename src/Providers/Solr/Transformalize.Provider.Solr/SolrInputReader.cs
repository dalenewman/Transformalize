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

using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;
using SolrNet.Impl;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Order = SolrNet.Order;

namespace Transformalize.Providers.Solr {
   public class SolrInputReader : IRead {

      private const string PhrasePattern = @"\""(?>[^""]+|\""(?<number>)|\""(?<-number>))*(?(number)(?!))\""";
      private static readonly Regex PhraseRegex = new Regex(PhrasePattern, RegexOptions.Compiled);

      private readonly ISolrReadOnlyOperations<Dictionary<string, object>> _solr;
      private readonly InputContext _context;
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
         _fieldNames = new Collection<string>(fields.Select(f => f.Name).ToList());
      }

      public IEnumerable<IRow> Read() {

         int counter = 0;
         var query = SolrQuery.All;
         var filterQueries = new Collection<ISolrQuery>();
         var facetQueries = new Collection<ISolrFacetQuery>();

         if (_context.Entity.Filter.Any()) {
            var queries = new Collection<ISolrQuery>();

            foreach (var filter in _context.Entity.Filter.Where(f => f.Type == "search" && f.Value != "*")) {
               if (filter.Field == string.Empty) {
                  _context.Debug(() => "Adding query expression: {filter.Expression}");
                  queries.Add(new SolrQuery(filter.Expression));
               } else {
                  foreach (var term in Terms(filter.Value)) {
                     _context.Debug(() => $"Adding {filter.Field} field query: {term}");
                     queries.Add(new SolrQueryByField(filter.Field, term) { Quoted = false });
                  }
               }
            }

            query = queries.Any() ? new SolrMultipleCriteriaQuery(queries, "AND") : SolrQuery.All;

            foreach (var filter in _context.Entity.Filter.Where(f => f.Type == "filter")) {
               if (filter.Field == string.Empty) {
                  _context.Debug(() => "Adding filter query expression: {filter.Expression}");
                  filterQueries.Add(new SolrQuery(filter.Expression));
               } else {
                  if (filter.Value != "*") {
                     foreach (var term in Terms(filter.Value)) {
                        _context.Debug(() => $"Adding {filter.Field} field filter query: {term}");
                        filterQueries.Add(new SolrQueryByField(filter.Field, term) { Quoted = false });
                     }
                  }
               }
            }

            foreach (var filter in _context.Entity.Filter.Where(f => f.Type == "facet")) {
               facetQueries.Add(new SolrFacetFieldQuery(filter.Field) {
                  MinCount = filter.Min,
                  Limit = filter.Size
               });
               if (filter.Value != "*") {
                  if (filter.Value.IndexOf(',') > 0) {
                     _context.Debug(() => $"Adding {filter.Field} field list query: {filter.Value}");
                     filterQueries.Add(new SolrQueryInList(filter.Field, filter.Value.Split(new[] { ',' })));
                  } else {
                     filterQueries.Add(new SolrQueryByField(filter.Field, filter.Value));
                  }
               }
            }
         }

         var sortOrder = new Collection<SortOrder>();
         if (_context.Entity.Order.Any()) {
            foreach (var orderBy in _context.Entity.Order) {
               if (_context.Entity.TryGetField(orderBy.Field, out var field)) {
                  var name = field.SortField.ToLower();
                  sortOrder.Add(new SortOrder(name, orderBy.Sort == "asc" ? Order.ASC : Order.DESC));
               }
            }
         }
         sortOrder.Add(new SortOrder("score", Order.DESC));

         if (_context.Entity.IsPageRequest()) {

            SolrQueryResults<Dictionary<string, object>> page = null;

            try {
               page = _solr.Query(
                   query,
                   new QueryOptions {
                      StartOrCursor = new StartOrCursor.Start(_context.Entity.Page * _context.Entity.Size - _context.Entity.Size),
                      Rows = _context.Entity.Size,
                      Fields = _fieldNames,
                      OrderBy = sortOrder,
                      FilterQueries = filterQueries,
                      Facet = new FacetParameters { Queries = facetQueries, Sort = false }
                   }
               );
            } catch (SolrConnectionException ex) {

               var msg = GetErrorMessage(ex.Message);
               if (msg == null) {
                  _context.Error(ex, ex.Message);
               } else {
                  _context.Error(msg);
               }
               yield break;
            }

            TransferFacetsToMaps(page);
            _context.Entity.Hits = page.NumFound > int.MaxValue ? int.MaxValue : (int) page.NumFound;

            foreach (var row in page.Select(x => DocToRow(_rowFactory.Create(), _fields, x))) {
               ++counter;
               yield return row;
            }

            yield break;
         }

         var readSize = _context.Entity.ReadSize > 0 ? _context.Entity.ReadSize : 500;
         var version = SolrVersionParser.ParseVersion(_context);

         // using the cursor is twice as fast
         if (version.Major > 4 || version.Major == 4 && version.Minor >= 7) {

            string uniqueKey;
            if (_context.Entity.GetAllFields().Any(f => f.PrimaryKey && !f.System)) {
               uniqueKey = _context.Entity.GetAllFields().First(f => f.PrimaryKey && !f.System).Name;
            } else {
               var key = new SolrSchemaReader(_context.Connection, _solr).Read().Entities.First().GetAllFields().FirstOrDefault(f => f.PrimaryKey);
               if (key == null) {
                  _context.Error($"Can't find unique key for {_context.Connection}.");
                  yield break;
               }
               uniqueKey = key.Name;
               _context.Debug(()=>$"Had to query a primary key: {uniqueKey}.");
            }

            if (sortOrder.All(s => s.FieldName != uniqueKey)) {
               sortOrder.Add(new SortOrder(uniqueKey, Order.ASC));
            }

            SolrQueryResults<Dictionary<string, object>> part = null;

            try {
               part = _solr.Query(
                   query,
                   new QueryOptions {
                      StartOrCursor = new StartOrCursor.Cursor("*"),
                      Rows = readSize,
                      Fields = _fieldNames,
                      OrderBy = sortOrder,
                      FilterQueries = filterQueries,
                      Facet = new FacetParameters { Queries = facetQueries, Sort = false }
                   }
               );
            } catch (SolrConnectionException ex) {

               var msg = GetErrorMessage(ex.Message);
               if (msg == null) {
                  _context.Error(ex, ex.Message);
               } else {
                  _context.Error(msg);
               }

               yield break;
            }

            TransferFacetsToMaps(part);
            _context.Entity.Hits = part.NumFound > int.MaxValue ? int.MaxValue : (int)part.NumFound;

            foreach (var row in part.Select(r => DocToRow(_rowFactory.Create(), _fields, r))) {
               ++counter;
               yield return row;
            }

            if (part.Count == part.NumFound) {
               yield break;
            }

            while (counter < part.NumFound) {

               part = _solr.Query(
                   query,
                   new QueryOptions {
                      StartOrCursor = part.NextCursorMark,
                      Rows = readSize,
                      Fields = _fieldNames,
                      OrderBy = sortOrder,
                      FilterQueries = filterQueries,
                      Facet = new FacetParameters { Queries = facetQueries, Sort = false }
                   }
               );

               if (part.Count == 0) {
                  yield break;
               }

               foreach (var row in part.Select(r => DocToRow(_rowFactory.Create(), _fields, r))) {
                  ++counter;
                  yield return row;
               }
            }

         } else {  // just regular paging

            SolrQueryResults<Dictionary<string, object>> part;
            try {
               part = _solr.Query(
                   query,
                   new QueryOptions {
                      Rows = readSize,
                      Fields = _fieldNames,
                      OrderBy = sortOrder,
                      FilterQueries = filterQueries,
                      Facet = new FacetParameters { Queries = facetQueries, Sort = false }
                   }
               );
            } catch (SolrConnectionException ex) {

               var msg = GetErrorMessage(ex.Message);
               if (msg == null) {
                  _context.Error(ex, ex.Message);
               } else {
                  _context.Error(msg);
               }

               yield break;
            }

            TransferFacetsToMaps(part);
            
            _context.Entity.Hits = part.NumFound > int.MaxValue ? int.MaxValue : (int)part.NumFound;

            if (part.Count == part.NumFound) {
               yield break;
            }

            foreach (var row in part.Select(r => DocToRow(_rowFactory.Create(), _fields, r))) {
               ++counter;
               yield return row;
            }

            // tradition paging 
            var pages = part.NumFound / readSize;
            for (var p = 1; p <= pages; p++) {
               part = _solr.Query(
                   query,
                   new QueryOptions {
                      StartOrCursor = new StartOrCursor.Start(p * readSize),
                      Rows = readSize,
                      Fields = _fieldNames,
                      OrderBy = sortOrder,
                      FilterQueries = filterQueries,
                      Facet = new FacetParameters { Queries = facetQueries, Sort = false }
                   }
               );

               if (part.Count == 0) {
                  yield break;
               }

               foreach (var row in part.Select(r => DocToRow(_rowFactory.Create(), _fields, r))) {
                  ++counter;
                  yield return row;
               }
            }
         }
      }

      private string GetErrorMessage(string xml) {
         XPathDocument doc = new XPathDocument(new StringReader(xml));
         var nagivator = doc.CreateNavigator();
         var maybe = nagivator.SelectSingleNode("/response/lst[@name='error']/str[@name='msg']/text()");
         if(maybe != null && maybe.Value != null) {
            var message = maybe.Value;
            var index = message.IndexOf("Was expecting one of");
            if (index > 0) {
               message = message.Substring(0, index);
            }
            if (message.StartsWith("org.apache.solr.search.SyntaxError:")) {
               message = message.Substring(35);
            }
            return message;
         }

         return xml;
      }

      private void TransferFacetsToMaps(AbstractSolrQueryResults<Dictionary<string, object>> result) {
         foreach (var filter in _context.Entity.Filter.Where(f => f.Type == "facet")) {
            if (result.FacetFields.ContainsKey(filter.Field)) {
               var facet = result.FacetFields[filter.Field];
               var map = _context.Process.Maps.First(m => m.Name == filter.Map);
               foreach (var f in facet) {
                  map.Items.Add(new MapItem { From = $"{f.Key} ({f.Value})", To = f.Key });
               }
            }
         }
      }

      private static IRow DocToRow(IRow row, Field[] fields, IReadOnlyDictionary<string, object> doc) {
         foreach (var field in fields) {
            if (doc.ContainsKey(field.Name)) {
               row[field] = doc[field.Name];
            }
         }
         return row;
      }

      private static IEnumerable<string> Terms(string value, string delimiter = " ") {

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
         foreach (var match in PhraseRegex.Matches(processedValue)) {
            phrases.Add(match.ToString());
            processedValue = processedValue.Replace(match.ToString(), string.Empty).Trim(delimiter.ToCharArray());
         }

         if (processedValue.Length > 0)
            phrases.AddRange(processedValue.Split(delimiter.ToCharArray()));
         return phrases;
      }
   }
}