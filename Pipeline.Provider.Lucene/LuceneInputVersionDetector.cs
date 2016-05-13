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
using Lucene.Net.Search;
using Pipeline.Context;
using Pipeline.Contracts;

namespace Pipeline.Provider.Lucene {
    public class LuceneInputVersionDetector : IInputVersionDetector {
        private readonly InputContext _context;
        private readonly SearcherFactory _searcherFactory;

        public LuceneInputVersionDetector(InputContext context, SearcherFactory searcherFactory) {
            _context = context;
            _searcherFactory = searcherFactory;
        }

        public object Detect() {
            if (string.IsNullOrEmpty(_context.Entity.Version))
                return null;

            using (var searcher = _searcherFactory.Create()) {
                var version = _context.Entity.GetVersionField();

                _context.Debug(() => $"Detecting max input version: {_context.Connection.Folder}:{version.Name}.");

                var hits = searcher.Search(new MatchAllDocsQuery(), null, 1,
                    new Sort(new SortField(version.Name, LuceneConversion.TypeSort(version.Type), true))
                );

                if (hits.TotalHits > 0) {
                    var doc = searcher.Doc(hits.ScoreDocs[0].Doc);
                    var value = doc.Get(version.Name);
                    _context.Debug(() => $"Found value: {value}");
                    return version.Convert(value);
                }
            }

            _context.Debug(() => "Did not find max input version");
            return null;

        }
    }
}
