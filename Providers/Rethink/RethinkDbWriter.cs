using RethinkDb;
using System.Collections.Generic;
using Transformalize.Context;
using Transformalize.Contracts;
using System.Linq;
using Transformalize.Configuration;
using System;
using Transformalize.Extensions;

namespace Transformalize.Provider.RethinkDB {
    public class RethinkDbWriter : IWrite {

        InputContext _input;
        OutputContext _output;
        readonly IConnectionFactory _factory;
        readonly Field[] _insertFields;
        readonly Field[] _updateFields;

        public RethinkDbWriter(InputContext input, OutputContext output, IConnectionFactory factory) {
            _input = input;
            _output = output;
            _factory = factory;
            _insertFields = output.OutputFields.Where(f => f.Type != "byte[]").ToArray();
            _updateFields = _insertFields.Where(f => f.Name != Constants.TflKey).ToArray();
        }

        public void Write(IEnumerable<IRow> rows) {
            var databaseName = _output.Connection.Database;
            var tableName = _output.Entity.Alias;
            var keyField = _output.Entity.GetPrimaryKey().First();
            var keyName = keyField.Alias;
            var keyIsNumeric = keyField.IsNumeric();
            var hashField = _output.Entity.TflHashCode();
            var hashName = hashField.Alias;
            var surrogateKeyField = _output.Entity.TflKey();
            var surrogateKeyName = surrogateKeyField.Alias;
            var conn = _factory.Get();
            var t = Query.Db(databaseName).Table<Dictionary<string, object>>(tableName);

            if (_output.Process.Mode == "init") {
                var result = conn.Run(t.Insert(rows.Select(r => _insertFields.ToDictionary(f => f.Alias, f => r[f]))));
                _output.Entity.Inserts = result.Inserted;
            } else {

                foreach (var part in rows.Partition(_output.Entity.InsertSize)) {
                    var expanded = part.ToArray();
                    var inserts = new List<IRow>();
                    var updates = new List<IRow>();

                    if (keyIsNumeric) {
                        var keys = expanded.Select(r => Convert.ToDouble(r[keyField])).ToArray();
                        var lookUp = new Dictionary<double, Dictionary<string, object>>();
                        foreach (var item in conn.Run(t.GetAll(keys))) {
                            lookUp[Convert.ToDouble(item[keyName])] = item;
                        }
                        foreach (var row in expanded) {
                            var key = Convert.ToDouble(row[keyField]);
                            if (lookUp.ContainsKey(key)) {
                                var source = (int)row[hashField];
                                var destination = (int) lookUp[key][hashName];
                                if (source != destination) {
                                    row[surrogateKeyField] = (int) lookUp[key][surrogateKeyName];
                                    updates.Add(row);
                                }
                            } else {
                                inserts.Add(row);
                            }
                        }

                    } else {
                        var keys = expanded.Select(r => r[keyField].ToString()).ToArray();
                        var lookUp = new Dictionary<string, Dictionary<string, object>>();
                        foreach (var item in conn.Run(t.GetAll(keys))) {
                            lookUp[item[keyName].ToString()] = item;
                        }
                        foreach (var row in expanded) {
                            var key = row[keyField].ToString();
                            if (lookUp.ContainsKey(key)) {
                                var source = (int)row[hashField];
                                var destination = (int) lookUp[key][hashName];
                                if (source != destination) {
                                    row[surrogateKeyField] = (int)lookUp[key][surrogateKeyName];
                                    updates.Add(row);
                                }
                            } else {
                                inserts.Add(row);
                            }
                        }

                    }

                    if (inserts.Count > 0) {
                        var result = conn.Run(t.Insert(inserts.Select(r => _insertFields.ToDictionary(f => f.Alias, f => r[f])), Conflict.Error));
                        _output.Entity.Inserts += result.Inserted;
                    }

                    if (updates.Count > 0) {

                        var result = conn.Run(t.Insert(updates.Select(r => _insertFields.ToDictionary(f => f.Alias, f => r[f])), Conflict.Update));
                        _output.Entity.Updates += result.Updated + result.Replaced;

                    }

                }
            }

            if (_output.Entity.Inserts > 0) {
                _output.Info($"{_output.Entity.Inserts} inserts into output");
            }
            if (_output.Entity.Updates > 0) {
                _output.Info($"{_output.Entity.Updates} updates to output");
            }

        }
    }
}
