using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Lucene.Net.Document;
using Transformalize.Libs.Lucene.Net.Index;
using Transformalize.Libs.Lucene.Net.QueryParser;
using Transformalize.Libs.Lucene.Net.Search;
using Transformalize.Libs.Lucene.Net.Util;
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
            foreach (var row in rows) {
                var doc = new Document();

                if (_deleteFirst) {
                    //var parser = new MultiFieldQueryParser(_luceneConnection.LuceneVersion(), _entity.PrimaryKey.Aliases().ToArray(), LuceneAnalyzerFactory.Create("keyword", _luceneConnection.Version)) { DefaultOperator = QueryParser.AND_OPERATOR };
                    var r = row;
                    //var queryString = string.Join(" AND ", _entity.PrimaryKey.Aliases().Select(a => string.Format("{0}:\"{1}\"", a.ToLower(), r[a].ToString())));
                    //var query = parser.Parse(queryString);
                    writer.DeleteDocuments(_entity.PrimaryKey.Select(f=>new Term(f.AliasLower,r[f.Alias].ToString())).ToArray());
                }

                foreach (var pair in _map) {
                    var name = pair.Key;
                    var field = pair.Value.Key;
                    var searchType = pair.Value.Value;

                    if (!searchType.Store && !searchType.Index)
                        continue;

                    doc.Add(LuceneIndexWriterFactory.GetAbstractField(field.SimpleType, name, searchType.Store, searchType.Index, row[field.Alias]));
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