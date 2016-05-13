#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Pipeline.Configuration;
using SolrNet;
using SolrNet.Commands.Parameters;
using Order = SolrNet.Order;

namespace Pipeline.Provider.Solr.Ext {
    public static class SolrExtensions {
        public static string BuildSolrUrl(this Connection cn) {
            var builder = new UriBuilder(cn.Server.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? cn.Server : "http://" + cn.Server);
            if (cn.Port > 0) {
                builder.Port = cn.Port;
            }
            builder.Path = (cn.Path == string.Empty ? cn.Core : cn.Path + "/" + cn.Core);
            return builder.ToString();
        }

        public static object GetMaxValue(this ISolrReadOnlyOperations<Dictionary<string, object>> solr, string field, AbstractSolrQuery baseQuery = null) {
            var result = solr.Query(
                baseQuery ?? SolrQuery.All,
                new QueryOptions {
                    StartOrCursor = new StartOrCursor.Start(0),
                    Rows = 1,
                    Fields = new List<string> { field },
                    OrderBy = new List<SortOrder> { new SortOrder(field, Order.DESC) }
                });
            return result.NumFound > 0 ? result[0][field] : null;
        }

        public static int GetCount(this ISolrReadOnlyOperations<Dictionary<string, object>> solr, AbstractSolrQuery baseQuery = null) {
            var result = solr.Query(
                baseQuery ?? SolrQuery.All,
                new QueryOptions {
                    StartOrCursor = new StartOrCursor.Start(0),
                    Rows = 0,
                    Fields = new Collection<string>()
                });
            return result.NumFound;
        }

    }
}
