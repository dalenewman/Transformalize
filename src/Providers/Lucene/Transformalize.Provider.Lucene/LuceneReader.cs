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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Transformalize.Contracts;
using Field = Transformalize.Configuration.Field;
using Lucene.Net.Util;
using Lucene.Net.QueryParsers.Classic;

namespace Transformalize.Providers.Lucene {
   public class LuceneReader : IRead, IDisposable {

      private const LuceneVersion V = LuceneVersion.LUCENE_48;
      private readonly IConnectionContext _context;
      private readonly IEnumerable<Field> _fields;
      private readonly SearcherFactory _searcherFactory;
      private readonly Analyzer _analyzer;
      private readonly IndexReaderFactory _readerFactory;
      private readonly IRowFactory _rowFactory;
      private readonly ReadFrom _readFrom;

      public LuceneReader(
          IConnectionContext context,
          IEnumerable<Field> fields,
          SearcherFactory searcherFactory,
          Analyzer analyzer,
          IndexReaderFactory readerFactory,
          IRowFactory rowFactory,
          ReadFrom readFrom) {
         _context = context;
         _fields = fields;
         _searcherFactory = searcherFactory;
         _analyzer = analyzer;
         _readerFactory = readerFactory;
         _rowFactory = rowFactory;
         _readFrom = readFrom;
      }

      public void Dispose() {
         if (_searcherFactory != null) {
            _searcherFactory.Dispose();
         }
      }

      public IEnumerable<IRow> Read() {

         using (var reader = _readerFactory.Create()) {
            var numDocs = reader.NumDocs;
            var selector = new DocumentStoredFieldVisitor(_fields.Select(f => f.Name).ToArray());
            var searcher = _searcherFactory.Create();

            // read from input?  consider filters, and field names
            if (_readFrom == ReadFrom.Input) {

               if (_context.Entity.Filter.Any()) {
                  var queryFields = _context.Entity.Filter.Select(f => f.Field).ToArray();
                  var query = string.Join(" ", _context.Entity.Filter.Select(f => "(" + (string.IsNullOrEmpty(f.Expression) ? f.Field + ":" + f.Value : f.Expression) + ") " + f.Continuation.ToUpper()));
                  query = query.Remove(query.Length - 3);
                  var topFieldCollector = TopFieldCollector.Create(Sort.INDEXORDER, numDocs, false, false, false, false);

                  searcher.Search(new MultiFieldQueryParser(V, queryFields, _analyzer).Parse(query), topFieldCollector);

                  TopDocs topDocs = topFieldCollector.GetTopDocs();

                  if (topDocs == null) {
                     yield break;
                  }

                  for (var i = 0; i < topDocs.TotalHits; i++) {
                     var row = _rowFactory.Create();
                     searcher.Doc(i, selector);
                     foreach (var field in _fields) {
                        row[field] = field.Convert(selector.Document.Get(field.Name));
                     }
                     yield return row;
                  }
               } else {

                  for (var i = 0; i < numDocs; i++) {
                     //if (reader.IsDeleted(i))
                     //   continue;
                     reader.Document(i, selector);
                     var row = _rowFactory.Create();
                     foreach (var field in _fields) {
                        row[field] = field.Convert(selector.Document.Get(field.Name));
                     }
                     yield return row;
                  }
               }

            } else {  // read from output? consider tfldeleted and field aliases

               var tflDeleted = _context.Entity.TflDeleted();
               var collector = TopFieldCollector.Create(Sort.INDEXORDER, numDocs, false, false, false, false);
               searcher.Search(LuceneConversion.TypeSearch(tflDeleted, tflDeleted.Alias, false), collector);

               var topDocs = collector.GetTopDocs();

               if (topDocs == null) {
                  yield break;
               }

               for (var i = 0; i < topDocs.TotalHits; i++) {
                  var row = _rowFactory.Create();
                  searcher.Doc(i, selector);
                  foreach (var field in _fields) {
                     row[field] = field.Convert(selector.Document.Get(field.Alias));
                  }
                  yield return row;
               }
            }
         }
      }

      public Task<IEnumerable<IRow>> ReadAsync(CancellationToken token = default) { return Task.FromResult(Read()); }
   }
}