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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Cfg.Net.Ext;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;


namespace Pipeline.Provider.Ado {
    public class AdoMapReader : IMapReader {
        private readonly IConnectionFactory _connectionFactory;

        public AdoMapReader(IConnectionFactory connectionFactory) {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<MapItem> Read(PipelineContext context) {
            var items = new List<MapItem>();
            var map = context.Process.Maps.First(m => m.Name == context.Transform.Map);
            var connection = context.Process.Connections.First(cn => cn.Name == map.Connection);
            using (var cn = _connectionFactory.GetConnection()) {
                cn.Open();
                var cmd = cn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = connection.RequestTimeout;
                cmd.CommandText = map.Query;
                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        var mapItem = new MapItem().WithDefaults();
                        mapItem.From = reader.IsDBNull(0) ? null : reader[0];
                        mapItem.To = reader.IsDBNull(1) ? null : reader[1];
                        items.Add(mapItem);
                    }
                }
            }
            return items;
        }
    }
}
