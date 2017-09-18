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
using System.Data;
using System.Dynamic;
using System.Linq;
using Dapper;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Providers.Ado {
    public class AdoMapReader : IMapReader {
        private readonly IContext _context;
        private readonly IConnectionFactory _connectionFactory;
        private readonly string _mapName;

        public AdoMapReader(IContext context, IConnectionFactory connectionFactory, string mapName) {
            _context = context;
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

                IDataReader reader;

                if (cmd.CommandText.Contains("@")) {
                    var parameters = new ExpandoObject();
                    var editor = (IDictionary<string, object>)parameters;
                    var active = _context.Process.GetActiveParameters();
                    foreach (var name in new AdoParameterFinder().Find(cmd.CommandText).Distinct().ToList()) {
                        var match = active.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                        if (match != null) {
                            editor[match.Name] = match.Convert(match.Value);
                        }
                    }
                    reader = cn.ExecuteReader(cmd.CommandText, parameters);
                } else {
                    reader = cn.ExecuteReader(cmd.CommandText);
                }

                using (reader) {
                    while (reader.Read()) {
                        if (reader.FieldCount > 0) {
                            if (reader.FieldCount > 1) {
                                var mapItem = new MapItem {
                                    From = reader.IsDBNull(0) ? null : reader[0],
                                    To = reader.IsDBNull(1) ? null : reader[1]
                                };
                                items.Add(mapItem);
                            } else {
                                var mapItem = new MapItem {
                                    From = reader.IsDBNull(0) ? null : reader[0],
                                    To = reader.IsDBNull(0) ? null : reader[0]
                                };
                                items.Add(mapItem);
                            }
                        }
                    }
                }
            }
            return items;
        }
    }
}
