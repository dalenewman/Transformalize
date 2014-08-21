using System.Collections.Generic;
using Transformalize.Libs.Lucene.Net.Analysis;
using Transformalize.Libs.Lucene.Net.Index;
using Transformalize.Libs.NLog;

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
            using (var dir = LuceneIndexDirectoryFactory.Create(connection, entity)) {
                Analyzer defaultAnalyzer = new KeywordAnalyzer();

                var analyzer = new PerFieldAnalyzerWrapper(defaultAnalyzer);
                foreach (var field in GetFields(entity, connection.Version)) {
                    analyzer.AddAnalyzer(field.Key, field.Value);
                }
                return new IndexWriter(dir, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
            }
        }

    }
}