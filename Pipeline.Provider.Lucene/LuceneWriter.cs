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
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Field = Transformalize.Configuration.Field;
using LuceneField = Lucene.Net.Documents.Field;

namespace Transformalize.Provider.Lucene {
    public class LuceneWriter : IWrite {

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

        public static AbstractField CreateField(FieldSearchType field, object value) {
            var s = field.SearchType.Store ? LuceneField.Store.YES : LuceneField.Store.NO;
            AbstractField abstractField;
            switch (field.Field.Type) {
                case "byte":
                    abstractField = new NumericField(field.Alias, s, field.SearchType.Index).SetIntValue(Convert.ToInt32(value));
                    break;
                case "short":
                case "int16":
                    abstractField = new NumericField(field.Alias, s, field.SearchType.Index).SetIntValue(Convert.ToInt32(value));
                    break;
                case "int":
                case "int32":
                    abstractField = new NumericField(field.Alias, s, field.SearchType.Index).SetIntValue((int)value);
                    break;
                case "int64":
                case "long":
                    abstractField = new NumericField(field.Alias, s, field.SearchType.Index).SetLongValue((long)value);
                    break;
                case "double":
                    abstractField = new NumericField(field.Alias, s, field.SearchType.Index).SetDoubleValue((double)value);
                    break;
                case "decimal":
                    abstractField = new LuceneField(field.Alias, ((decimal)value).ToString(field.Field.DecimalFormat), s, field.SearchType.Index ? LuceneField.Index.NOT_ANALYZED_NO_NORMS : LuceneField.Index.NO, LuceneField.TermVector.NO);
                    break;
                case "float":
                case "single":
                    abstractField = new NumericField(field.Alias, s, field.SearchType.Index).SetFloatValue((float)value);
                    break;
                case "bool":
                case "boolean":
                    abstractField = new LuceneField(field.Alias, (bool)value ? "1" : "0", s, LuceneField.Index.NOT_ANALYZED_NO_NORMS);
                    break;
                case "datetime":
                    abstractField = new LuceneField(field.Alias, DateTools.DateToString((DateTime)value, DateTools.Resolution.MILLISECOND), s, field.SearchType.Index ? LuceneField.Index.NOT_ANALYZED_NO_NORMS : LuceneField.Index.NO, LuceneField.TermVector.NO);
                    break;
                case "rowversion":
                case "byte[]":
                    abstractField = field.SearchType.Index ?
                        new LuceneField(field.Alias, Utility.BytesToHexString((byte[])value), s, LuceneField.Index.NOT_ANALYZED_NO_NORMS) :
                        new LuceneField(field.Alias, (byte[])value, s);
                    break;
                case "string":
                    var iString = field.SearchType.Index ? (
                            field.SearchType.Analyzer.Equals("keyword") ?
                            (field.SearchType.Norms ? LuceneField.Index.NOT_ANALYZED : LuceneField.Index.NOT_ANALYZED_NO_NORMS) :
                            (field.SearchType.Norms ? LuceneField.Index.ANALYZED : LuceneField.Index.ANALYZED_NO_NORMS)
                         ) :
                            LuceneField.Index.NO;

                    abstractField = new LuceneField(field.Alias, value.ToString(), s, iString);
                    break;
                default:
                    var i = field.SearchType.Index ?
                        (field.SearchType.Norms ? LuceneField.Index.NOT_ANALYZED : LuceneField.Index.NOT_ANALYZED_NO_NORMS) :
                        LuceneField.Index.NO;

                    abstractField = new LuceneField(field.Alias, value.ToString(), s, i);
                    break;
            }
            return abstractField;
        }

        public void Write(IEnumerable<IRow> rows) {
            var tflKey = _output.Entity.TflKey();
            using (var searcher = _searcherFactory.Create()) {
                using (var writer = _writerFactory.Create()) {
                    foreach (var row in rows) {
                        var tflId = string.Concat(_primaryKey.Select(pk => row[pk].ToString()));
                        var doc = new Document();
                        foreach (var field in _fieldSearchTypes.Where(field => field.SearchType.Store || field.SearchType.Index)) {
                            doc.Add(CreateField(field, row[field.Field]));
                        }
                        doc.Add(new LuceneField("TflId", tflId, LuceneField.Store.YES, LuceneField.Index.NOT_ANALYZED_NO_NORMS));
                        if (_output.Entity.IsFirstRun) {
                            writer.AddDocument(doc);
                        } else {
                            var term = new Term("TflId", tflId);
                            var hits = searcher.Search(new TermQuery(term), null, 1);
                            if (hits.TotalHits > 0) {
                                var old = searcher.Doc(hits.ScoreDocs[0].Doc);
                                doc.RemoveField(tflKey.Alias);
                                doc.Add(new NumericField(tflKey.Alias, LuceneField.Store.YES, true).SetIntValue(Convert.ToInt32(old.Get(tflKey.Alias))));
                                writer.UpdateDocument(term, doc);
                            } else {
                                writer.AddDocument(doc);
                            }
                        }
                    }
                    writer.Commit();
                    writer.Optimize();
                }
            }

        }
    }
}