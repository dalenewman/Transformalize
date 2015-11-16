using System;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Logging;
using Lucene.Net.Documents;

namespace Transformalize.Main.Providers.Lucene {

    public class LuceneWriter {
        private static readonly List<string> Analyzers = new List<string> {
            "standard",
            "simple",
            "whitespace",
            "keyword",
            string.Empty
        };
        public static readonly Dictionary<string, int> SortMap = new Dictionary<string, int>() {
            {"string", SortField.STRING},
            {"byte", SortField.BYTE},
            {"int16", SortField.SHORT},
            {"short", SortField.SHORT},
            {"int32", SortField.INT},
            {"int", SortField.INT},
            {"int64", SortField.LONG},
            {"long", SortField.LONG},
            {"double", SortField.DOUBLE},
            {"float", SortField.FLOAT},
            {"date", SortField.LONG},
            {"datetime", SortField.LONG},
            {"bool", SortField.STRING},
            {"boolean", SortField.STRING},
            {"*", SortField.STRING}
        };

        public static Query CreateQuery(string name, string type, object value) {
            Query query;
            switch (type) {
                case "byte":
                    query = NumericRangeQuery.NewIntRange(name, Convert.ToInt32(value), Convert.ToInt32(value), true, true);
                    break;
                case "int16":
                    query = NumericRangeQuery.NewIntRange(name, Convert.ToInt32(value), Convert.ToInt32(value), true, true);
                    break;
                case "int":
                    var intValue = (int)value;
                    query = NumericRangeQuery.NewIntRange(name, intValue, intValue, true, true);
                    break;
                case "int32":
                    var int32Value = (int)value;
                    query = NumericRangeQuery.NewIntRange(name, int32Value, int32Value, true, true);
                    break;
                case "int64":
                    var int64Value = (long)value;
                    query = NumericRangeQuery.NewLongRange(name, int64Value, int64Value, true, true);
                    break;
                case "long":
                    var longValue = (long)value;
                    query = NumericRangeQuery.NewLongRange(name, longValue, longValue, true, true);
                    break;
                case "double":
                    var doubleValue = (double)value;
                    query = NumericRangeQuery.NewDoubleRange(name, doubleValue, doubleValue, true, true);
                    break;
                case "float":
                    var floatValue = (float)value;
                    query = NumericRangeQuery.NewFloatRange(name, floatValue, floatValue, true, true);
                    break;
                case "datetime":
                    var datetimeValue = ((DateTime)value).Ticks;
                    query = NumericRangeQuery.NewLongRange(name, datetimeValue, datetimeValue, true, true);
                    break;
                case "string":
                    query = new TermQuery(new Term((string)value));
                    break;
                case "byte[]":
                    query = new TermQuery(new Term(Common.BytesToHexString((byte[])value)));
                    break;
                case "rowversion":
                    query = new TermQuery(new Term(Common.BytesToHexString((byte[])value)));
                    break;
                default:
                    query = new TermQuery(new Term(name, Convert.ToString(value)));
                    break;
            }
            return query;
        }

        public static AbstractField CreateField(string name, string type, SearchType searchType, object value) {
            var s = searchType.Store ? global::Lucene.Net.Documents.Field.Store.YES : global::Lucene.Net.Documents.Field.Store.NO;
            AbstractField abstractField;
            switch (type) {
                case "byte":
                    abstractField = new NumericField(name, s, searchType.Index).SetIntValue(Convert.ToInt32(value));
                    break;
                case "int16":
                    abstractField = new NumericField(name, s, searchType.Index).SetIntValue(Convert.ToInt32(value));
                    break;
                case "int":
                    abstractField = new NumericField(name, s, searchType.Index).SetIntValue((int)value);
                    break;
                case "int32":
                    abstractField = new NumericField(name, s, searchType.Index).SetIntValue((int)value);
                    break;
                case "int64":
                    abstractField = new NumericField(name, s, searchType.Index).SetLongValue((long)value);
                    break;
                case "long":
                    abstractField = new NumericField(name, s, searchType.Index).SetLongValue((long)value);
                    break;
                case "double":
                    abstractField = new NumericField(name, s, searchType.Index).SetDoubleValue((double)value);
                    break;
                case "float":
                    abstractField = new NumericField(name, s, searchType.Index).SetFloatValue((float)value);
                    break;
                case "datetime":
                    abstractField = new NumericField(name, s, searchType.Index).SetLongValue(((DateTime)value).Ticks);
                    break;
                case "byte[]":
                    abstractField = searchType.Index ?
                        new global::Lucene.Net.Documents.Field(name, Common.BytesToHexString((byte[])value), s, global::Lucene.Net.Documents.Field.Index.NOT_ANALYZED_NO_NORMS) :
                        new global::Lucene.Net.Documents.Field(name, (byte[])value, global::Lucene.Net.Documents.Field.Store.YES);
                    break;
                case "rowversion":
                    abstractField = searchType.Index ?
                        new global::Lucene.Net.Documents.Field(name, Common.BytesToHexString((byte[])value), s, global::Lucene.Net.Documents.Field.Index.NOT_ANALYZED_NO_NORMS) :
                        new global::Lucene.Net.Documents.Field(name, (byte[])value, global::Lucene.Net.Documents.Field.Store.YES);
                    break;
                case "string":
                    var iString = searchType.Index ? (
                            searchType.Analyzer.Equals("keyword") ?
                            (searchType.Norms ? global::Lucene.Net.Documents.Field.Index.NOT_ANALYZED : global::Lucene.Net.Documents.Field.Index.NOT_ANALYZED_NO_NORMS) :
                            (searchType.Norms ? global::Lucene.Net.Documents.Field.Index.ANALYZED : global::Lucene.Net.Documents.Field.Index.ANALYZED_NO_NORMS)
                         ) :
                            global::Lucene.Net.Documents.Field.Index.NO;
                    abstractField = new global::Lucene.Net.Documents.Field(name, value.ToString(), s, iString);
                    break;
                default:
                    var i = searchType.Index ?
                        (searchType.Norms ? global::Lucene.Net.Documents.Field.Index.NOT_ANALYZED : global::Lucene.Net.Documents.Field.Index.NOT_ANALYZED_NO_NORMS) :
                        global::Lucene.Net.Documents.Field.Index.NO;
                    abstractField = new global::Lucene.Net.Documents.Field(name, value.ToString(), s, i);
                    break;

            }
            return abstractField;

        }

        public static Dictionary<string, KeyValuePair<Field, SearchType>> CreateFieldMap(Entity entity) {
            var fields = new Dictionary<string, KeyValuePair<Field, SearchType>>();
            foreach (var field in entity.OutputFields()) {
                foreach (var searchType in field.SearchTypes) {
                    if (fields.ContainsKey(field.Alias)) {
                        fields[field.Alias + searchType.Name] = new KeyValuePair<Field, SearchType>(field, searchType);
                    } else {
                        fields[field.Alias] = new KeyValuePair<Field, SearchType>(field, searchType);
                    }
                }
            }
            if (!fields.ContainsKey("TflBatchId")) {
                fields.Add("TflBatchId", new KeyValuePair<Field, SearchType>(new Field("int", "0", FieldType.None, true, "0") { Name = "tflbatchid" }, new SearchType() { Analyzer = "keyword", Index = true, MultiValued = false, Name = "keyword", Store = true }));
            }
            if (entity.Delete && !fields.ContainsKey("TflDeleted")) {
                fields.Add("TflDeleted", new KeyValuePair<Field, SearchType>(new Field("boolean", "false", FieldType.None, true, "0") { Name = "tfldeleted" }, new SearchType() { Analyzer = "keyword", Index = true, MultiValued = false, Name = "keyword", Store = true }));
            }
            return fields;
        }


        private static Dictionary<string, Analyzer> GetFields(Entity entity, string version, ILogger logger) {
            var fields = new Dictionary<string, Analyzer>();
            foreach (var field in entity.OutputFields()) {
                foreach (var searchType in field.SearchTypes) {
                    if (Analyzers.Contains(searchType.Analyzer)) {
                        if (fields.ContainsKey(field.Alias)) {
                            fields[field.Alias + searchType.Name] = LuceneAnalyzerFactory.Create(searchType.Analyzer, version);
                        } else {
                            fields[field.Alias] = LuceneAnalyzerFactory.Create(searchType.Analyzer, version);
                        }
                    } else {
                        logger.EntityWarn(entity.Name, "Analyzer '{0}' specified in search type '{1}' is not supported.  Lucene is limited to standard, simple, keyword, or whitespace.", searchType.Analyzer, searchType.Name);
                        if (!fields.ContainsKey(field.Alias)) {
                            fields[field.Alias] = LuceneAnalyzerFactory.Create(searchType.Analyzer, version);
                        }
                    }
                }
            }
            if (!fields.ContainsKey("TflBatchId")) {
                fields.Add("TflBatchId", new KeywordAnalyzer());
            }
            if (entity.Delete && !fields.ContainsKey("TflDeleted")) {
                fields.Add("TflDeleted", new KeywordAnalyzer());
            }
            return fields;
        }

        public static Query CreateQuery(IEnumerable<Field> primaryKey, Row row) {
            var combinedQuery = new BooleanQuery();
            foreach (var key in primaryKey) {
                combinedQuery.Add(CreateQuery(key.Alias, key.SimpleType, row[key.Alias]), Occur.MUST);
            }
            return combinedQuery;
        }

        public static Document CreateDocument(Dictionary<string, KeyValuePair<Field, SearchType>> fieldMap, Row row) {
            var doc = new Document();
            foreach (var pair in fieldMap) {
                var name = pair.Key;
                var field = pair.Value.Key;
                var searchType = pair.Value.Value;

                if (!searchType.Store && !searchType.Index)
                    continue;

                doc.Add(CreateField(name, field.SimpleType, searchType, row[field.Alias]));
            }
            return doc;

        }

        public static IndexWriter Create(AbstractConnection connection, Process process, Entity entity) {
            using (var dir = LuceneDirectoryFactory.Create(connection, entity)) {
                Analyzer defaultAnalyzer = new KeywordAnalyzer();
                if (process.SearchTypes.ContainsKey("default")) {
                    defaultAnalyzer = LuceneAnalyzerFactory.Create(process.SearchTypes["default"].Analyzer, connection.Version);
                }

                var analyzer = new PerFieldAnalyzerWrapper(defaultAnalyzer);
                foreach (var field in GetFields(entity, connection.Version, connection.Logger)) {
                    analyzer.AddAnalyzer(field.Key, field.Value);
                }
                return new IndexWriter(dir, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
            }
        }

        public static IndexWriter Create(AbstractConnection connection, Entity entity) {
            var dir = LuceneDirectoryFactory.Create(connection, entity);
            Analyzer defaultAnalyzer = new KeywordAnalyzer();

            var analyzer = new PerFieldAnalyzerWrapper(defaultAnalyzer);
            foreach (var field in GetFields(entity, connection.Version, connection.Logger)) {
                analyzer.AddAnalyzer(field.Key, field.Value);
            }
            return new IndexWriter(dir, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
        }

    }
}