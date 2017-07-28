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
using SolrNet;
using SolrNet.Commands.Parameters;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Provider.Solr.Ext;

namespace Transformalize.Provider.Solr {
    public class SolrOutputProvider : IOutputProvider {

        private readonly OutputContext _context;
        private readonly ISolrReadOnlyOperations<Dictionary<string, object>> _solr;

        public SolrOutputProvider(OutputContext context, ISolrReadOnlyOperations<Dictionary<string, object>> solr) {
            _context = context;
            _solr = solr;
        }

        public object GetMaxVersion() {
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

        public void Initialize() {
            throw new NotImplementedException();
        }

        public int GetNextTflBatchId() {
            // query and set Context.Entity.BatchId (max of TflBatchId)
            var batchIdField = _context.Entity.TflBatchId();
            var batchId = _solr.GetMaxValue(batchIdField.Alias);
            return batchId != null ? Convert.ToInt32(batchId) + 1 : 0;
        }

        public int GetMaxTflKey() {
            // query and set Context.Entity.Identity (max of Identity)
            var identityField = _context.Entity.TflKey();
            var identity = _solr.GetMaxValue(identityField.Alias);
            return identity != null ? Convert.ToInt32(identity) : 0;

        }

        public void Start() {
            throw new NotImplementedException();
        }

        public void End() {
            throw new NotImplementedException();
        }

        public void Write(IEnumerable<IRow> rows) {
            throw new NotImplementedException();
        }

        public void Delete() {
            throw new NotImplementedException();
        }

        public IEnumerable<IRow> ReadKeys() {
            throw new NotImplementedException();
        }

        public IEnumerable<IRow> Match(IEnumerable<IRow> rows) {
            throw new NotImplementedException();
        }

        public void Dispose() {
        }
    }
}