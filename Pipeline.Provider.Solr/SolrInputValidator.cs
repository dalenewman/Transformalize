#region license
// Transformalize
// Copyright 2013 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
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
using SolrNet;

namespace Pipeline.Provider.Solr {

    public class SolrInputValidator : IInputValidator {

        private readonly InputContext _context;
        private readonly ISolrReadOnlyOperations<Dictionary<string, object>> _solr;

        public SolrInputValidator(InputContext context, ISolrReadOnlyOperations<Dictionary<string, object>> solr) {
            _context = context;
            _solr = solr;
        }

        public ActionResponse Execute() {
            var response = new ActionResponse();

            var schema = _solr.GetSchema(_context.Connection.SchemaFileName);

            foreach (var field in _context.InputFields) {
                var solrField = schema.FindSolrFieldByName(field.Name);
                if (!solrField.IsStored) {
                    response.Code = 500;
                    response.Content += $"The solr field {solrField.Name} is not stored, so it can not be retrieved.  You must remove the field or add input='false' to it." + Environment.NewLine;
                }
            }
            response.Content = response.Content.TrimEnd(Environment.NewLine.ToCharArray());
            return response;
        }
    }
}
