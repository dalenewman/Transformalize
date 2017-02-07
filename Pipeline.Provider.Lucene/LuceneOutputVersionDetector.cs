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
using Lucene.Net.Search;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Provider.Lucene {
    public class LuceneOutputVersionDetector : IVersionDetector {
        private readonly OutputContext _context;
        private readonly SearcherFactory _searcherFactory;

        public LuceneOutputVersionDetector(OutputContext context, SearcherFactory searcherFactory) {
            _context = context;
            _searcherFactory = searcherFactory;
        }

        public object Detect() {

            if (string.IsNullOrEmpty(_context.Entity.Version))
                return null;

            var version = _context.Entity.GetVersionField();

            _context.Debug(() => $"Detecting max output version: {_context.Connection.Folder}.{_context.Entity.Alias}.{version.Alias}.");

            var tflDeleted = _context.Entity.TflDeleted();
            var sort = new Sort(new SortField(version.Alias, LuceneConversion.TypeSort(version.Type), true));
            using (var searcher = _searcherFactory.Create()) {
                var hits = searcher.Search(LuceneConversion.TypeSearch(tflDeleted, tflDeleted.Alias, false), null, 1, sort);

                if (hits.TotalHits > 0) {
                    var doc = searcher.Doc(hits.ScoreDocs[0].Doc);
                    var value = doc.Get(version.Alias);
                    _context.Debug(() => $"Found value: {value}");
                    return version.Convert(value);
                }
            }

            _context.Debug(() => "Did not find max output version");
            return null;
        }
    }
}