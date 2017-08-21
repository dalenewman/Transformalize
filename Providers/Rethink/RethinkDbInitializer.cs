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

using System.Linq;
using RethinkDb;
using Transformalize.Actions;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.RethinkDB {
    public class RethinkDbInitializer : IInitializer {

        readonly InputContext _input;
        readonly OutputContext _output;
        readonly IConnectionFactory _factory;

        public RethinkDbInitializer(InputContext input, OutputContext output, IConnectionFactory connectionFactory) {
            _input = input;
            _output = output;
            _factory = connectionFactory;
        }

        public ActionResponse Execute() {

            var response = new ActionResponse();
            var database = _output.Connection.Database;
            var table = _output.Entity.Alias;
            var conn = _factory.Get();
            DmlResponse res;

            if (!conn.Run(Query.DbList()).Contains(database)) {
                res = conn.Run(Query.DbCreate(database));
                _output.Info($"Created {database}");
            }

            IDatabaseQuery db = Query.Db(database);

            if (conn.Run(db.TableList()).Contains(table)) {
                res = conn.Run(db.TableDrop(table));
                _output.Info($"Dropped {table}");
            }

            var keys = _output.Entity.GetPrimaryKey();
            if(keys.Count() > 1) {
                _output.Error("You can't create a composite primary key in RethinkDB.");
            } else {
                if (keys.Any()) {
                    conn.Run(db.TableCreate(table, primaryKey: keys.First().Alias));
                } else {
                    conn.Run(db.TableCreate(table));
                }
                _output.Info($"Created {table}");
            }

            return response;
        }


    }
}
