using Transformalize.Actions;
using Transformalize.Context;
using Transformalize.Contracts;
using RethinkDb;
using System.Linq;

namespace Transformalize.Provider.RethinkDB {
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
