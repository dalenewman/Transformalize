using RethinkDb;
using System.Collections.Generic;
using Transformalize.Context;
using Transformalize.Contracts;
using System.Linq;
using Transformalize.Configuration;
using System;

namespace Transformalize.Provider.RethinkDB {
    public class RethinkDbWriter : IWrite {

        InputContext _input;
        OutputContext _output;
        readonly IConnectionFactory _factory;
        readonly Field[] _fields;
        readonly Field[] _fieldsWithoutKeys;

        public RethinkDbWriter(InputContext input, OutputContext output, IConnectionFactory factory) {
            _input = input;
            _output = output;
            _factory = factory;
            _fields = output.OutputFields.Where(f => f.Type != "byte[]").ToArray();
            _fieldsWithoutKeys = _fields.Where(f => !f.PrimaryKey && f.Name != Constants.TflKey).ToArray();
        }

        public void Write(IEnumerable<IRow> rows) {
            var databaseName = _output.Connection.Database;
            var tableName = _output.Entity.Alias;
            var keyField = _output.Entity.GetPrimaryKey().First();
            var keyName = keyField.Alias;
            var hashField = _output.Entity.TflHashCode();
            var hashName = hashField.Alias;
            var conn = _factory.Get();
            var t = Query.Db(databaseName).Table<Dictionary<string, object>>(tableName);

            if (_output.Process.Mode == "init") {
                var result = conn.Run(t.Insert(rows.Select(r => _fields.ToDictionary(f => f.Alias, f => r[f]))));
                _output.Entity.Inserts = result.Inserted;
            } else {
                foreach (var r in rows) {

                    var existing = conn.Run(t.Get(Convert.ToDouble(r[keyField])));

                    if(existing == null) {
                        var result = conn.Run(t.Insert(_fields.ToDictionary(f => f.Alias, f => r[f])));
                        _output.Entity.Inserts += result.Inserted;
                    } else {
                        var result = conn.Run(t.Get(Convert.ToDouble(r[keyField])).Update(x=> _fieldsWithoutKeys.ToDictionary(f => f.Alias, f => r[f])));
                        _output.Entity.Updates += (result.Updated + result.Replaced);
                    }

                }
            }

            _output.Info($"{_output.Entity.Inserts} inserts into output");
            _output.Info($"{_output.Entity.Updates} updates to output");

        }
    }
}
