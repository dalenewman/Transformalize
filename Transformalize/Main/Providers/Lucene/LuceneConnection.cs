using System;
using Transformalize.Configuration;
using Transformalize.Libs.Lucene.Net.Analysis;
using Transformalize.Libs.Lucene.Net.Document;
using Transformalize.Libs.Lucene.Net.Index;
using Transformalize.Libs.Lucene.Net.QueryParser;
using Transformalize.Libs.Lucene.Net.Search;
using Transformalize.Libs.Rhino.Etl.Operations;
using Sort = Transformalize.Libs.Lucene.Net.Search.Sort;
using Version = Transformalize.Libs.Lucene.Net.Util.Version;

namespace Transformalize.Main.Providers.Lucene {
    public class LuceneConnection : AbstractConnection {

        public LuceneConnection(ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {
            Type = ProviderType.Lucene;
            IsDatabase = true;
        }

        public override int NextBatchId(string processName) {
            if (!TflBatchRecordsExist(processName)) {
                return 1;
            }
            using (var searcher = LuceneSearcherFactory.Create(this, TflBatchEntity(processName))) {

                var query = new TermQuery(new Term("process", processName));
                var sort = new Sort(new SortField("id", SortField.INT, true));
                var hits = searcher.Search(query, null, 1, sort);

                if (hits.TotalHits <= 0)
                    return 1;

                var doc = searcher.Doc(0);
                return Convert.ToInt32(doc.GetField("id").StringValue) + 1;
            }
        }

        public override void WriteEndVersion(Process process, AbstractConnection input, Entity entity, bool force = false) {
            if (entity.Updates + entity.Inserts <= 0 && !force)
                return;

            var versionType = entity.Version == null ? "string" : entity.Version.SimpleType;
            var end = entity.End ?? new DefaultFactory().Convert(entity.End, versionType);

            using (var dir = LuceneDirectoryFactory.Create(this, TflBatchEntity(entity.ProcessName))) {
                using (var writer = new IndexWriter(dir, new KeywordAnalyzer(), IndexWriter.MaxFieldLength.UNLIMITED)) {
                    var doc = new Document();
                    doc.fields.Add(new NumericField("id", Libs.Lucene.Net.Document.Field.Store.YES, true).SetIntValue(entity.TflBatchId));
                    doc.fields.Add(new Libs.Lucene.Net.Document.Field("process", entity.ProcessName, Libs.Lucene.Net.Document.Field.Store.YES, Libs.Lucene.Net.Document.Field.Index.NOT_ANALYZED_NO_NORMS));
                    doc.fields.Add(new Libs.Lucene.Net.Document.Field("connection", input.Name, Libs.Lucene.Net.Document.Field.Store.YES, Libs.Lucene.Net.Document.Field.Index.NOT_ANALYZED_NO_NORMS));
                    doc.fields.Add(new Libs.Lucene.Net.Document.Field("entity", entity.Alias, Libs.Lucene.Net.Document.Field.Store.YES, Libs.Lucene.Net.Document.Field.Index.NOT_ANALYZED_NO_NORMS));
                    doc.fields.Add(new NumericField("updates", Libs.Lucene.Net.Document.Field.Store.YES, true).SetLongValue(entity.Updates));
                    doc.fields.Add(new NumericField("inserts", Libs.Lucene.Net.Document.Field.Store.YES, true).SetLongValue(entity.Inserts));
                    doc.fields.Add(new NumericField("deletes", Libs.Lucene.Net.Document.Field.Store.YES, true).SetLongValue(entity.Deletes));
                    doc.fields.Add(LuceneWriter.CreateField("version", versionType, new SearchType { Analyzer = "keyword" }, end));
                    doc.fields.Add(new Libs.Lucene.Net.Document.Field("version_type", versionType, Libs.Lucene.Net.Document.Field.Store.YES, Libs.Lucene.Net.Document.Field.Index.NOT_ANALYZED_NO_NORMS));
                    doc.fields.Add(new NumericField("tflupdate", Libs.Lucene.Net.Document.Field.Store.YES, true).SetLongValue(DateTime.UtcNow.Ticks));
                    writer.AddDocument(doc);
                    writer.Commit();
                    writer.Optimize();
                }
            }
        }

        public override IOperation ExtractCorrespondingKeysFromOutput(Entity entity) {
            return new LuceneKeysExtractAll(this, entity, input: false);
        }

        public override IOperation ExtractAllKeysFromOutput(Entity entity) {
            return new LuceneKeysExtractAll(this, entity, input: false);
        }

        public override IOperation ExtractAllKeysFromInput(Process process, Entity entity) {
            return new LuceneKeysExtractAll(this, entity, input: true);
        }

        public override IOperation Insert(Process process, Entity entity) {
            return new LuceneLoadOperation(this, entity);
        }

        public override IOperation Update(Entity entity) {
            return new LuceneLoadOperation(this, entity, true);
        }

        public override void LoadBeginVersion(Entity entity) {
            using (var searcher = LuceneSearcherFactory.Create(this, TflBatchEntity(entity.ProcessName))) {

                var parser = new MultiFieldQueryParser(LuceneVersion(), new[] { "process", "entity" }, new KeywordAnalyzer());
                var query = parser.Parse(string.Format("process:\"{0}\" AND entity:\"{1}\"", entity.ProcessName, entity.Alias));
                var sort = new Sort(new SortField("id", SortField.INT, true));
                var hits = searcher.Search(query, null, 1, sort);

                entity.HasRange = hits.TotalHits > 0;

                if (!entity.HasRange)
                    return;

                var doc = searcher.Doc(0);
                var type = doc.GetField("version_type").StringValue;
                entity.Begin = Common.GetObjectConversionMap()[type](doc.GetField("version").StringValue);
            }
        }

        public override void LoadEndVersion(Entity entity) {
            using (var searcher = LuceneSearcherFactory.Create(this, entity)) {
                var query = new MatchAllDocsQuery();
                var sortType = LuceneWriter.SortMap.ContainsKey(entity.Version.SimpleType) ? LuceneWriter.SortMap[entity.Version.SimpleType] : LuceneWriter.SortMap["*"];
                var sort = new Sort(new SortField(entity.Version.Alias, sortType, true));
                var hits = searcher.Search(query, null, 1, sort);

                entity.HasRows = hits.TotalHits > 0;

                if (!entity.HasRows)
                    return;

                var doc = searcher.Doc(0);
                entity.End = Common.GetObjectConversionMap()[entity.Version.SimpleType](doc.Get(entity.Version.Alias));
            }
        }

        public override Fields GetEntitySchema(Process process, Entity entity, bool isMaster = false) {
            throw new NotImplementedException();
        }

        public override IOperation Delete(Entity entity) {
            return new LuceneEntityDelete(this, entity);
        }

        public override IOperation Extract(Process process, Entity entity, bool firstRun) {
            return new LuceneExtract(this, entity);
        }

        public static Version GetLuceneVersion(string version) {
            Version v;
            if (version == Common.DefaultValue || !Enum.TryParse(version, true, out v)) {
                v = Libs.Lucene.Net.Util.Version.LUCENE_30;
            }
            return v;
        }

        public Version LuceneVersion() {
            return GetLuceneVersion(this.Version);
        }

    }
}