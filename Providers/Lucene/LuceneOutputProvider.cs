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
using Lucene.Net.Search;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.Lucene {
    public class LuceneOutputProvider : IOutputProvider {

        private readonly OutputContext _context;
        private readonly Searcher _searcher;

        public LuceneOutputProvider(OutputContext context, SearcherFactory searcherFactory) {
            _context = context;
            _searcher = searcherFactory.Create();
        }

        public void Delete() {
            throw new NotImplementedException();
        }

        public object GetMaxVersion() {

            if (string.IsNullOrEmpty(_context.Entity.Version))
                return null;

            var version = _context.Entity.GetVersionField();

            _context.Debug(() => $"Detecting max output version: {_context.Connection.Folder}.{_context.Entity.Alias}.{version.Alias}.");

            var tflDeleted = _context.Entity.TflDeleted();
            var sort = new Sort(new SortField(version.Alias, LuceneConversion.TypeSort(version.Type), true));
            var hits = _searcher.Search(LuceneConversion.TypeSearch(tflDeleted, tflDeleted.Alias, false), null, 1, sort);

            if (hits.TotalHits > 0) {
                var doc = _searcher.Doc(hits.ScoreDocs[0].Doc);
                var value = doc.Get(version.Alias);
                _context.Debug(() => $"Found value: {value}");
                return version.Convert(value);
            }

            _context.Debug(() => "Did not find max output version");
            return null;
        }

        public void End() {
            throw new NotImplementedException();
        }

        public int GetNextTflBatchId() {
            var tflBatchId = _context.Entity.TflBatchId();
            var tflKey = _context.Entity.TflKey();
            var batchHits = _searcher.Search(new MatchAllDocsQuery(), null, 1,
                new Sort(new SortField(tflBatchId.Alias, LuceneConversion.TypeSort(tflBatchId.Type), true))
            );
            return (batchHits.TotalHits > 0 ? Convert.ToInt32(_searcher.Doc(batchHits.ScoreDocs[0].Doc).Get(tflBatchId.Alias)) : 0) + 1;
        }

        public int GetMaxTflKey() {
            var tflBatchId = _context.Entity.TflBatchId();
            var tflKey = _context.Entity.TflKey();
            var keyHits = _searcher.Search(new MatchAllDocsQuery(), null, 1,
                new Sort(new SortField(tflKey.Alias, LuceneConversion.TypeSort(tflKey.Type), true))
            );
            return (keyHits.TotalHits > 0 ? Convert.ToInt32(_searcher.Doc(keyHits.ScoreDocs[0].Doc).Get(tflKey.Alias)) : 0);
        }

        public void Initialize() {
            throw new NotImplementedException();
        }

        public IEnumerable<IRow> Match(IEnumerable<IRow> rows) {
            throw new NotImplementedException();
        }

        public IEnumerable<IRow> ReadKeys() {
            throw new NotImplementedException();
        }

        public void Start() {
            throw new NotImplementedException();
        }

        public void Write(IEnumerable<IRow> rows) {
            throw new NotImplementedException();
        }

        public void Dispose() {
            if (_searcher != null) {
                _searcher.Dispose();
            }
        }
    }
}