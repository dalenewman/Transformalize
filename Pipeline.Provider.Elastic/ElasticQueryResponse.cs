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
using System.Linq;
using Elasticsearch.Net;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;

namespace Pipeline.Provider.Elastic {

    public class QueryResponse {

        private readonly IRowFactory _rowFactory;
        private readonly Dictionary<string, Field> _fields;
        public ElasticsearchResponse<DynamicResponse> Response { get; set; }

        public QueryResponse(InputContext context, IRowFactory rowFactory, ElasticsearchResponse<DynamicResponse> response) {
            _rowFactory = rowFactory;
            _fields = context.InputFields.ToDictionary(k => k.Name, v => v);
            Response = response;
        }

        public IEnumerable<dynamic> Aggregations => Response.Body["aggregations"];


    }
}