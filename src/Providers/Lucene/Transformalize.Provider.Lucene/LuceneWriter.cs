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
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Transformalize.Context;
using Transformalize.Contracts;
using Field = Transformalize.Configuration.Field;
using LuceneField = Lucene.Net.Documents.Field;

namespace Transformalize.Providers.Lucene {
   public class LuceneWriter : IWrite, IDisposable {

      private readonly IndexWriterFactory _writerFactory;
      private readonly SearcherFactory _searcherFactory;
      private readonly List<FieldSearchType> _fieldSearchTypes;
      private readonly OutputContext _output;
      private readonly Field[] _primaryKey;

      public LuceneWriter(OutputContext output, IndexWriterFactory writerFactory, SearcherFactory searcherFactory) {
         _output = output;
         _writerFactory = writerFactory;
         _searcherFactory = searcherFactory;
         _fieldSearchTypes = new FieldSearchTypes(output.Process, output.OutputFields).ToList();
         _primaryKey = output.Entity.GetPrimaryKey();
      }

      public static IIndexableField CreateField(FieldSearchType field, object value) {
         var s = field.SearchType.Store ? LuceneField.Store.YES : LuceneField.Store.NO;
         LuceneField f;
         switch (field.Field.Type) {
            case "byte":
               f = new Int32Field(field.Field.Alias, Convert.ToInt32(value), s);
               break;
            case "short":
            case "int16":
               f = new Int32Field(field.Field.Alias, Convert.ToInt32(value), s);
               break;
            case "int":
            case "int32":
               f = new Int32Field(field.Field.Alias, (int)value, s);
               break;
            case "int64":
            case "long":
               f = new Int64Field(field.Field.Alias, (long)value, s);
               break;
            case "double":
               f = new DoubleField(field.Field.Alias, (double)value, s);
               break;
            case "decimal":
               f = new StringField(field.Field.Alias, ((decimal)value).ToString(LuceneConversion.CalculateDecimalFormat(field.Field.Precision, field.Field.Scale)), s);
               break;
            case "float":
            case "single":
               f = new SingleField(field.Field.Alias, (float)value, s);
               break;
            case "bool":
            case "boolean":
               f = new StringField(field.Field.Alias, (bool)value ? "1" : "0", s);
               break;
            case "datetime":
               f = new StringField(field.Field.Alias, DateTools.DateToString((DateTime)value, DateResolution.MILLISECOND), s);
               break;
            case "rowversion":
            case "byte[]":
               f = new TextField(field.Field.Alias, Utility.BytesToHexString((byte[])value), s);
               break;
            case "string":
               var iString = field.SearchType.Index ? (
                       field.SearchType.Analyzer.Equals("keyword") ?
                       (field.SearchType.Norms ? LuceneField.Index.NOT_ANALYZED : LuceneField.Index.NOT_ANALYZED_NO_NORMS) :
                       (field.SearchType.Norms ? LuceneField.Index.ANALYZED : LuceneField.Index.ANALYZED_NO_NORMS)
                    ) :
                       LuceneField.Index.NO;

               f = new LuceneField(field.Field.Alias, value.ToString(), s, iString);
               break;
            default:
               var i = field.SearchType.Index ?
                   (field.SearchType.Norms ? LuceneField.Index.NOT_ANALYZED : LuceneField.Index.NOT_ANALYZED_NO_NORMS) :
                   LuceneField.Index.NO;

               f = new LuceneField(field.Field.Alias, value.ToString(), s, i);
               break;
         }
         return f;
      }

      public void Dispose() {
         if (_searcherFactory != null) {
            _searcherFactory.Dispose();
         }
      }

      public Task WriteAsync(IEnumerable<IRow> rows, CancellationToken token = default) { Write(rows); return Task.CompletedTask; }

      public void Write(IEnumerable<IRow> rows) {
         var tflKey = _output.Entity.TflKey();
         var searcher = _searcherFactory.Create();
         using (var writer = _writerFactory.Create()) {
            foreach (var row in rows) {
               var tflId = string.Concat(_primaryKey.Select(pk => row[pk].ToString()));
               var doc = new Document();
               foreach (var field in _fieldSearchTypes.Where(field => field.SearchType.Store || field.SearchType.Index)) {
                  doc.Add(CreateField(field, row[field.Field]));
               }
               doc.Add(new LuceneField("TflId", tflId, LuceneField.Store.YES, LuceneField.Index.NOT_ANALYZED_NO_NORMS));
               if (_output.Process.Mode == "init") {
                  writer.AddDocument(doc);
                  _output.Entity.Inserts += 1;
               } else {
                  var term = new Term("TflId", tflId);
                  var hits = searcher.Search(new TermQuery(term), null, 1);
                  if (hits.TotalHits > 0) {
                     var old = searcher.Doc(hits.ScoreDocs[0].Doc);
                     doc.RemoveField(tflKey.Alias);
                     doc.Add(new Int32Field(tflKey.Alias, Convert.ToInt32(old.Get(tflKey.Alias)), LuceneField.Store.YES));
                     writer.UpdateDocument(term, doc);
                     _output.Entity.Updates += 1;
                  } else {
                     writer.AddDocument(doc);
                     _output.Entity.Inserts += 1;
                  }
               }
            }
            writer.Commit();
         }
      }
   }
}