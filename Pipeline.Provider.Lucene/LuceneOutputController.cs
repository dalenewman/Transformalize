#region license
// Transformalize
// A Configurable ETL solution specializing in incremental denormalization.
// Copyright 2013 Dale Newman
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
using System.Diagnostics;
using Lucene.Net.Search;
using Pipeline.Contracts;
using Lucene.Net.Index;
using Pipeline.Context;

namespace Pipeline.Provider.Lucene {
    public class LuceneOutputController : BaseOutputController {
        private readonly SearcherFactory _searcherFactory;
        private readonly IndexReaderFactory _readerFactory;
        private readonly Stopwatch _stopWatch;

        public LuceneOutputController(
            OutputContext context,
            IAction initializer,
            IVersionDetector inputVersionDetector,
            IVersionDetector outputVersionDetector,
            SearcherFactory searcherFactory,
            IndexReaderFactory readerFactory
            ) : base(
                context,
                initializer,
                inputVersionDetector,
                outputVersionDetector
                ) {
            _searcherFactory = searcherFactory;
            _readerFactory = readerFactory;
            _stopWatch = new Stopwatch();
        }

        public override void End() {
            _stopWatch.Stop();
            Context.Info("Ending {0}", _stopWatch.Elapsed);
        }

        // get max tflbatchid, max tflkey
        public override void Start() {
            base.Start();
            var tflBatchId = Context.Entity.TflBatchId();
            var tflKey = Context.Entity.TflKey();
            using (var searcher = _searcherFactory.Create()) {
                var batchHits = searcher.Search(new MatchAllDocsQuery(), null, 1,
                    new Sort(new SortField(tflBatchId.Alias, LuceneConversion.TypeSort(tflBatchId.Type), true))
                );
                Context.Entity.BatchId = (batchHits.TotalHits > 0 ? System.Convert.ToInt32(searcher.Doc(batchHits.ScoreDocs[0].Doc).Get(tflBatchId.Alias)) : 0) + 1;

                var keyHits = searcher.Search(new MatchAllDocsQuery(), null, 1,
                    new Sort(new SortField(tflKey.Alias, LuceneConversion.TypeSort(tflKey.Type), true))
                );
                Context.Entity.Identity = (keyHits.TotalHits > 0 ? System.Convert.ToInt32(searcher.Doc(keyHits.ScoreDocs[0].Doc).Get(tflKey.Alias)) : 0);
            }
            Context.Debug(()=>$"Next {tflBatchId.Alias}: {Context.Entity.BatchId}.");
            Context.Debug(() => $"Last {tflKey.Alias}: {Context.Entity.Identity}.");

            using (var reader = _readerFactory.Create()) {
                Context.Entity.IsFirstRun = Context.Entity.MinVersion == null && reader.NumDocs() == 0;
            }
        }

    }
}
