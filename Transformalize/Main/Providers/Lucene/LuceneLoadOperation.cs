using System.Collections.Generic;
using Transformalize.Libs.Lucene.Net.Document;
using Transformalize.Libs.Lucene.Net.Search;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Main.Providers.Lucene {
    public class LuceneLoadOperation : AbstractOperation {
        private readonly LuceneConnection _luceneConnection;
        private readonly Entity _entity;
        private readonly bool _deleteFirst;
        private readonly Dictionary<string, KeyValuePair<Field, SearchType>> _map;
        private IndexSearcher _indexSearcher = null;

        public LuceneLoadOperation(LuceneConnection luceneConnection, Entity entity, bool deleteFirst = false) {
            _luceneConnection = luceneConnection;
            _entity = entity;
            _deleteFirst = deleteFirst;
            _map = LuceneIndexWriterFactory.GetFieldMap(entity);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            if (_deleteFirst)
                _indexSearcher = LuceneIndexSearcherFactory.Create(_luceneConnection, _entity);

            var writer = LuceneIndexWriterFactory.Create(_luceneConnection, _entity);
            foreach (var row in rows) {
                var doc = new Document();

                if (_deleteFirst) {
                    var combinedQuery = new BooleanQuery();
                    foreach (var key in _entity.PrimaryKey) {
                        combinedQuery.Add(LuceneIndexWriterFactory.GetAbstractQuery(key.AliasLower, key.SimpleType, row[key.Alias]), Occur.MUST);
                    }
                    var results = _indexSearcher.Search(combinedQuery, null, 1);
                    if (results.TotalHits > 0) {
                        writer.DeleteDocuments(combinedQuery);
                    }
                }

                foreach (var pair in _map) {
                    var name = pair.Key;
                    var field = pair.Value.Key;
                    var searchType = pair.Value.Value;

                    if (!searchType.Store && !searchType.Index)
                        continue;

                    doc.Add(LuceneIndexWriterFactory.GetAbstractField(name, field.SimpleType, searchType.Analyzer, searchType.Store, searchType.Index, row[field.Alias]));
                }
                writer.AddDocument(doc);
            }
            if (_deleteFirst) {
                _indexSearcher.Dispose();
            }
            writer.Commit();
            writer.Optimize();
            writer.Dispose();
            yield break;
        }
    }
}