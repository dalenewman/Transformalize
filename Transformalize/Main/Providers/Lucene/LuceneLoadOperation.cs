using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Lucene.Net.Document;
using Transformalize.Libs.Lucene.Net.Index;
using Transformalize.Libs.Lucene.Net.QueryParser;
using Transformalize.Libs.Lucene.Net.Search;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Main.Providers.Lucene {
    public class LuceneLoadOperation : AbstractOperation {
        private readonly LuceneConnection _luceneConnection;
        private readonly Entity _entity;
        private readonly bool _deleteFirst;
        private readonly Dictionary<string, KeyValuePair<Field, SearchType>> _map;

        public LuceneLoadOperation(LuceneConnection luceneConnection, Entity entity, bool deleteFirst = false) {
            _luceneConnection = luceneConnection;
            _entity = entity;
            _deleteFirst = deleteFirst;
            _map = LuceneIndexWriterFactory.GetFieldMap(entity);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            var writer = LuceneIndexWriterFactory.Create(_luceneConnection, _entity);
            var version = (Libs.Lucene.Net.Util.Version)Enum.Parse(typeof(Libs.Lucene.Net.Util.Version), _luceneConnection.Version, true);
            foreach (var row in rows) {
                var doc = new Document();

                foreach (var pair in _map) {
                    AbstractField abstractField;
                    var name = pair.Key;
                    var field = pair.Value.Key;
                    var searchType = pair.Value.Value;

                    if (!searchType.Store && !searchType.Index)
                        continue;

                    if (_deleteFirst) {
                        var parser = new MultiFieldQueryParser(version, _entity.PrimaryKey.Aliases().ToArray(), LuceneAnalyzerFactory.Create("keyword", _luceneConnection.Version)) { DefaultOperator = QueryParser.AND_OPERATOR };
                        var r = row;
                        var queryString = string.Join(" AND ", _entity.PrimaryKey.Aliases().Select(a => string.Format("{0}:{1}", a.ToLower(), r[a].ToString())));
                        var query = parser.Parse(queryString);
                        writer.DeleteDocuments(query);
                    }

                    var store = searchType.Store ? Libs.Lucene.Net.Document.Field.Store.YES : Libs.Lucene.Net.Document.Field.Store.NO;
                    var index = searchType.Index ? Libs.Lucene.Net.Document.Field.Index.ANALYZED : Libs.Lucene.Net.Document.Field.Index.NO;

                    switch (field.SimpleType) {
                        case "byte":
                            abstractField = new NumericField(name, store, searchType.Index).SetIntValue(Convert.ToInt32(row[field.Alias]));
                            break;
                        case "int16":
                            abstractField = new NumericField(name, store, searchType.Index).SetIntValue(Convert.ToInt32(row[field.Alias]));
                            break;
                        case "int":
                            abstractField = new NumericField(name, store, searchType.Index).SetIntValue((int)row[field.Alias]);
                            break;
                        case "int32":
                            abstractField = new NumericField(name, store, searchType.Index).SetIntValue((int)row[field.Alias]);
                            break;
                        case "int64":
                            abstractField = new NumericField(name, store, searchType.Index).SetLongValue((long)row[field.Alias]);
                            break;
                        case "long":
                            abstractField = new NumericField(name, store, searchType.Index).SetLongValue((long)row[field.Alias]);
                            break;
                        case "double":
                            abstractField = new NumericField(name, store, searchType.Index).SetDoubleValue((double)row[field.Alias]);
                            break;
                        case "float":
                            abstractField = new NumericField(name, store, searchType.Index).SetFloatValue((float)row[field.Alias]);
                            break;
                        case "datetime":
                            abstractField = new NumericField(name, store, searchType.Index).SetLongValue(((DateTime)row[field.Alias]).Ticks);
                            break;
                        default:
                            abstractField = new Libs.Lucene.Net.Document.Field(name, row[field.Alias].ToString(), store, index);
                            break;
                    }
                    doc.Add(abstractField);
                }
                writer.AddDocument(doc);
            }
            writer.Commit();
            writer.Optimize();
            writer.Dispose();
            yield break;
        }
    }
}