using System;
using Transformalize.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation.Configuration;
using Transformalize.Libs.Lucene.Net.Analysis;
using Transformalize.Libs.Lucene.Net.Document;
using Transformalize.Libs.Lucene.Net.Index;
using Transformalize.Libs.Lucene.Net.Search;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Libs.SolrNet.Utils;
using Transformalize.Operations;
using Transformalize.Operations.Transform;
using Sort = Transformalize.Libs.Lucene.Net.Search.Sort;

namespace Transformalize.Main.Providers.Lucene {
    public class LuceneConnection : AbstractConnection {

        public LuceneConnection(ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies) : base(element, dependencies) { }

        public override int NextBatchId(string processName) {
            if (!TflBatchRecordsExist(processName)) {
                return 1;
            }
            using (var searcher = LuceneIndexSearcherFactory.Create(this, TflBatchEntity(processName))) {
                var query = new MatchAllDocsQuery();
                var sort = new Sort(new SortField("id", SortField.INT, false));
                var hits = searcher.Search(query, null, 1, sort);

                if (hits.TotalHits <= 0)
                    return 1;

                var doc = searcher.Doc(0);
                return Convert.ToInt32(doc.GetField("id").StringValue) + 1;
            }
        }

        public override void WriteEndVersion(AbstractConnection input, Entity entity, bool force = false) {
            if (entity.Updates + entity.Inserts <= 0 && !force)
                return;

            var versionType = entity.Version == null ? "string" : entity.Version.SimpleType;
            var end = versionType.Equals("byte[]") || versionType.Equals("rowversion") ? Common.BytesToHexString((byte[])entity.End) : new DefaultFactory().Convert(entity.End, versionType).ToString();

            using (var dir = LuceneIndexDirectoryFactory.Create(this, TflBatchEntity(entity.ProcessName))) {
                using (var writer = new IndexWriter(dir, new KeywordAnalyzer(), IndexWriter.MaxFieldLength.UNLIMITED)) {
                    var doc = new Document();
                    doc.fields.Add(new NumericField("id", Libs.Lucene.Net.Document.Field.Store.YES, true).SetIntValue(entity.TflBatchId));
                    doc.fields.Add(new Libs.Lucene.Net.Document.Field("process", entity.ProcessName, Libs.Lucene.Net.Document.Field.Store.YES, Libs.Lucene.Net.Document.Field.Index.NOT_ANALYZED));
                    doc.fields.Add(new Libs.Lucene.Net.Document.Field("connection", input.Name, Libs.Lucene.Net.Document.Field.Store.YES, Libs.Lucene.Net.Document.Field.Index.NOT_ANALYZED));
                    doc.fields.Add(new Libs.Lucene.Net.Document.Field("entity", entity.Alias, Libs.Lucene.Net.Document.Field.Store.YES, Libs.Lucene.Net.Document.Field.Index.NOT_ANALYZED));
                    doc.fields.Add(new NumericField("updates", Libs.Lucene.Net.Document.Field.Store.YES, true).SetLongValue(entity.Updates));
                    doc.fields.Add(new NumericField("inserts", Libs.Lucene.Net.Document.Field.Store.YES, true).SetLongValue(entity.Inserts));
                    doc.fields.Add(new NumericField("deletes", Libs.Lucene.Net.Document.Field.Store.YES, true).SetLongValue(entity.Deletes));
                    doc.fields.Add(new Libs.Lucene.Net.Document.Field("version", end, Libs.Lucene.Net.Document.Field.Store.YES, Libs.Lucene.Net.Document.Field.Index.NOT_ANALYZED));
                    doc.fields.Add(new Libs.Lucene.Net.Document.Field("version_type", versionType, Libs.Lucene.Net.Document.Field.Store.YES, Libs.Lucene.Net.Document.Field.Index.NOT_ANALYZED));
                    doc.fields.Add(new NumericField("tflupdate", Libs.Lucene.Net.Document.Field.Store.YES, true).SetLongValue(DateTime.UtcNow.Ticks));
                    writer.AddDocument(doc);
                    writer.Commit();
                    writer.Optimize();
                }
            }
        }

        public override IOperation EntityOutputKeysExtract(Entity entity) {
            return new EmptyOperation();
            //todo: build LuceneEntityOutputKeysExtract
        }

        public override IOperation EntityOutputKeysExtractAll(Entity entity) {
            return new LuceneOutputKeysExtractAll(this, entity);
        }

        public override IOperation EntityBulkLoad(Entity entity) {
            return new LuceneLoadOperation(this, entity);
        }

        public override IOperation EntityBatchUpdate(Entity entity) {
            return new LuceneLoadOperation(this, entity, true);
        }

        public override void LoadBeginVersion(Entity entity) {
            throw new NotImplementedException();
        }

        public override void LoadEndVersion(Entity entity) {
            throw new NotImplementedException();
        }

        public override Fields GetEntitySchema(Process process, string name, string schema = "", bool isMaster = false) {
            throw new NotImplementedException();
        }


    }
}