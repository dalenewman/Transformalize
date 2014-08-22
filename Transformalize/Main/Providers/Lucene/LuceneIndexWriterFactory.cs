using System;
using System.Collections.Generic;
using System.Web;
using Transformalize.Libs.Lucene.Net.Analysis;
using Transformalize.Libs.Lucene.Net.Document;
using Transformalize.Libs.Lucene.Net.Index;
using Transformalize.Libs.Lucene.Net.Search;
using Transformalize.Libs.NLog;
using Transformalize.Libs.SolrNet.Utils;

namespace Transformalize.Main.Providers.Lucene {
    public class LuceneIndexWriterFactory {
        private static readonly Logger Log = LogManager.GetLogger("tfl");
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
            {"*", SortField.STRING}
        };

        public static AbstractField GetAbstractField(string type, string name, bool store, bool index, object value) {
            var s = store ? Libs.Lucene.Net.Document.Field.Store.YES : Libs.Lucene.Net.Document.Field.Store.NO;
            AbstractField abstractField;
            switch (type) {
                case "byte":
                    abstractField = new NumericField(name, s, index).SetIntValue(Convert.ToInt32(value));
                    break;
                case "int16":
                    abstractField = new NumericField(name, s, index).SetIntValue(Convert.ToInt32(value));
                    break;
                case "int":
                    abstractField = new NumericField(name, s, index).SetIntValue((int)value);
                    break;
                case "int32":
                    abstractField = new NumericField(name, s, index).SetIntValue((int)value);
                    break;
                case "int64":
                    abstractField = new NumericField(name, s, index).SetLongValue((long)value);
                    break;
                case "long":
                    abstractField = new NumericField(name, s, index).SetLongValue((long)value);
                    break;
                case "double":
                    abstractField = new NumericField(name, s, index).SetDoubleValue((double)value);
                    break;
                case "float":
                    abstractField = new NumericField(name, s, index).SetFloatValue((float)value);
                    break;
                case "datetime":
                    abstractField = new NumericField(name, s, index).SetLongValue(((DateTime)value).Ticks);
                    break;
                default:
                    var i = index ? Libs.Lucene.Net.Document.Field.Index.ANALYZED : Libs.Lucene.Net.Document.Field.Index.NO;
                    abstractField = new Libs.Lucene.Net.Document.Field(name, value.ToString(), s, i);
                    break;
            }
            return abstractField;

        }

        public static Dictionary<string, KeyValuePair<Field, SearchType>> GetFieldMap(Entity entity) {
            var fields = new Dictionary<string, KeyValuePair<Field, SearchType>>();
            foreach (var field in entity.OutputFields()) {
                var alias = field.Alias.ToLower();
                foreach (var searchType in field.SearchTypes) {
                    if (fields.ContainsKey(alias)) {
                        fields[alias + searchType.Name.ToLower()] = new KeyValuePair<Field, SearchType>(field, searchType);
                    } else {
                        fields[alias] = new KeyValuePair<Field, SearchType>(field, searchType);
                    }
                }
            }
            if (!fields.ContainsKey("tflbatchid")) {
                fields.Add("tflbatchid", new KeyValuePair<Field, SearchType>(new Field("int", "0", FieldType.None, true, "0") { Name = "tflbatchid" }, new SearchType() { Analyzer = "keyword", Index = true, MultiValued = false, Name = "keyword", Store = true }));
            }
            if (!fields.ContainsKey("tfldeleted")) {
                fields.Add("tfldeleted", new KeyValuePair<Field, SearchType>(new Field("boolean", "false", FieldType.None, true, "0") { Name = "tfldeleted" }, new SearchType() { Analyzer = "keyword", Index = true, MultiValued = false, Name = "keyword", Store = true }));
            }
            return fields;
        }


        public static Dictionary<string, Analyzer> GetFields(Entity entity, string version) {
            var fields = new Dictionary<string, Analyzer>();
            foreach (var field in entity.OutputFields()) {
                var alias = field.Alias.ToLower();
                foreach (var searchType in field.SearchTypes) {
                    var analyzer = searchType.Analyzer.ToLower();
                    if (Analyzers.Contains(analyzer)) {
                        if (fields.ContainsKey(alias)) {
                            fields[alias + searchType.Name.ToLower()] = LuceneAnalyzerFactory.Create(analyzer, version);
                        } else {
                            fields[alias] = LuceneAnalyzerFactory.Create(analyzer, version);
                        }
                    } else {
                        Log.Warn("Analyzer '{0}' specified in search type '{1}' is not supported.  Lucene is limited to standard, simple, keyword, or whitespace.", analyzer, searchType.Name);
                        if (!fields.ContainsKey(alias)) {
                            fields[alias] = LuceneAnalyzerFactory.Create(analyzer, version);
                        }
                    }
                }
            }
            if (!fields.ContainsKey("tflbatchid")) {
                fields.Add("tflbatchid", new KeywordAnalyzer());
            }
            if (!fields.ContainsKey("tfldeleted")) {
                fields.Add("tfldeleted", new KeywordAnalyzer());
            }
            return fields;
        }


        public static IndexWriter Create(AbstractConnection connection, Process process, Entity entity) {
            using (var dir = LuceneIndexDirectoryFactory.Create(connection, entity)) {
                Analyzer defaultAnalyzer = new KeywordAnalyzer();
                if (process.SearchTypes.ContainsKey("default")) {
                    defaultAnalyzer = LuceneAnalyzerFactory.Create(process.SearchTypes["default"].Analyzer, connection.Version);
                }

                var analyzer = new PerFieldAnalyzerWrapper(defaultAnalyzer);
                foreach (var field in GetFields(entity, connection.Version)) {
                    analyzer.AddAnalyzer(field.Key, field.Value);
                }
                return new IndexWriter(dir, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
            }
        }

        public static IndexWriter Create(AbstractConnection connection, Entity entity) {
            var dir = LuceneIndexDirectoryFactory.Create(connection, entity);
            Analyzer defaultAnalyzer = new KeywordAnalyzer();

            var analyzer = new PerFieldAnalyzerWrapper(defaultAnalyzer);
            foreach (var field in GetFields(entity, connection.Version)) {
                analyzer.AddAnalyzer(field.Key, field.Value);
            }
            return new IndexWriter(dir, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
        }

    }
}