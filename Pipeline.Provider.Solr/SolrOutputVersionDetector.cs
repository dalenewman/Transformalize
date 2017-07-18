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
using System.Collections.Generic;
using SolrNet;
using SolrNet.Commands.Parameters;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Provider.Solr {
    public class SolrOutputVersionDetector : IOutputVersionDetector {

        private readonly OutputContext _context;
        private readonly ISolrReadOnlyOperations<Dictionary<string, object>> _solr;

        public SolrOutputVersionDetector(OutputContext context, ISolrReadOnlyOperations<Dictionary<string, object>> solr) {
            _context = context;
            _solr = solr;
        }

        public object Detect() {
            if (string.IsNullOrEmpty(_context.Entity.Version))
                return null;

            var version = _context.Entity.GetVersionField();
            var versionName = version.Alias.ToLower();

            _context.Debug(() => $"Detecting Max Output Version: {_context.Connection.Database}.{versionName}.");

            var result = _solr.Query(
                _context.Entity.Delete ? new SolrQueryByField(_context.Entity.TflDeleted().Alias.ToLower(), "false") : SolrQuery.All,
                new QueryOptions {
                    StartOrCursor = new StartOrCursor.Start(0),
                    Rows = 1,
                    Fields = new List<string> { versionName },
                    OrderBy = new List<SortOrder> { new SortOrder(versionName, Order.DESC) }
                });

            var value = result.NumFound > 0 ? result[0][versionName] : null;
            if (value != null && value.GetType() != Constants.TypeSystem()[version.Type]) {
                value = version.Convert(value);
            }
            _context.Debug(() => $"Found value: {value ?? "null"}");
            return value;
        }
    }
}