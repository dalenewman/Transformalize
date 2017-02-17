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
using System.Data;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Provider.Ado {
    public class AdoMapReader : IMapReader {
        private readonly IConnectionFactory _connectionFactory;
        private readonly string _mapName;

        public AdoMapReader(IConnectionFactory connectionFactory, string mapName) {
            _connectionFactory = connectionFactory;
            _mapName = mapName;
        }

        public IEnumerable<MapItem> Read(IContext context) {
            var items = new List<MapItem>();
            var map = context.Process.Maps.First(m => m.Name == _mapName);
            var connection = context.Process.Connections.First(cn => cn.Name == map.Connection);
            using (var cn = _connectionFactory.GetConnection()) {
                cn.Open();
                var cmd = cn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = connection.RequestTimeout;
                cmd.CommandText = map.Query;
                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        var mapItem = new MapItem {
                            From = reader.IsDBNull(0) ? null : reader[0],
                            To = reader.IsDBNull(1) ? null : reader[1]
                        };
                        items.Add(mapItem);
                    }
                }
            }
            return items;
        }
    }
}
