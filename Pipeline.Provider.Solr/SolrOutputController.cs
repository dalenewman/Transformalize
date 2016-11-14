#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Provider.Solr.Ext;
using SolrNet;

namespace Pipeline.Provider.Solr {
    public class SolrOutputController : BaseOutputController {
        private readonly ISolrReadOnlyOperations<Dictionary<string, object>> _solr;

        public SolrOutputController(
            OutputContext context,
            IAction initializer,
            IVersionDetector inputVersionDetector,
            IVersionDetector outputVersionDetector,
            ISolrReadOnlyOperations<Dictionary<string, object>> solr
            ) : base(
                context,
                initializer,
                inputVersionDetector,
                outputVersionDetector
            ) {
            _solr = solr;
        }

        public override void Start() {
            base.Start();

            // query and set Context.Entity.BatchId (max of TflBatchId)
            var batchIdField = Context.Entity.TflBatchId();
            var batchId = _solr.GetMaxValue(batchIdField.Alias);
            if (batchId != null) {
                Context.Entity.BatchId = Convert.ToInt32(batchId) + 1;
            }

            // query and set Context.Entity.Identity (max of Identity)
            var identityField = Context.Entity.TflKey();
            var identity = _solr.GetMaxValue(identityField.Alias);
            if (identity != null) {
                Context.Entity.Identity = Convert.ToInt32(identity);
            }

            // query record count in output and use with MinVersion to determine Context.Entity.IsFirstRun
            Context.Entity.IsFirstRun = Context.Entity.MinVersion == null && _solr.GetCount() == 0;

        }
    }
}
