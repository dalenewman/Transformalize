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
using System.Linq;
using Dapper;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Providers.Ado {
    public class AdoEntityMatchingKeysReader : ITakeAndReturnRows {

        private readonly IConnectionContext _context;
        private readonly Field[] _keys;
        private readonly IConnectionFactory _cf;
        private readonly Field[] _fields;
        private readonly AdoRowCreator _rowCreator;
        private readonly Field _hashCode;
        private readonly string _tempTable;
        private readonly Field _deleted;

        public AdoEntityMatchingKeysReader(IConnectionContext context, IConnectionFactory cf, IRowFactory rowFactory) {

            _tempTable = (cf.AdoProvider == AdoProvider.SqlServer ? "#" : string.Empty) + context.Entity.GetExcelName();
            _context = context;
            _keys = context.Entity.GetPrimaryKey();
            _cf = cf;
            _hashCode = context.Entity.TflHashCode();
            _deleted = context.Entity.TflDeleted();
            _fields = new List<Field>(_keys) { _hashCode, _deleted }.ToArray();
            _rowCreator = new AdoRowCreator(context, rowFactory);
        }

        private string SqlDrop(string tempTable) {
            var sql = $"DROP TABLE {_cf.Enclose(tempTable)}";
            _context.Debug(() => sql);
            return sql;
        }

        private string SqlCreateKeysTable(string tempTable) {
            var columnsAndDefinitions = string.Join(",", _context.Entity.GetPrimaryKey().Select(f => _cf.Enclose(f.FieldName()) + " " + _cf.SqlDataType(f) + " NOT NULL"));
            var sql = $"CREATE {(_cf.AdoProvider == AdoProvider.SqlServer || _cf.AdoProvider == AdoProvider.SqlCe || _cf.AdoProvider == AdoProvider.Access ? string.Empty : "TEMPORARY ")}TABLE {_cf.Enclose(tempTable)}({columnsAndDefinitions})";
            _context.Debug(() => sql);
            return sql;
        }

        private string SqlQuery() {
            var names = string.Join(",", _keys.Select(f => "k." + _cf.Enclose(f.FieldName())));
            var table = _context.Entity.OutputTableName(_context.Process.Name);
            var joins = string.Join(" AND ", _keys.Select(f => "o." + _cf.Enclose(f.FieldName()) + " = k." + _cf.Enclose(f.FieldName())));
            var sql = $"SELECT {names},o.{_cf.Enclose(_hashCode.FieldName())},o.{_cf.Enclose(_deleted.FieldName())} FROM {_cf.Enclose(_tempTable)} k INNER JOIN {_cf.Enclose(table)} o ON ({joins})";
            _context.Debug(() => sql);
            return sql;
        }

        private string SqlInsertTemplate(IContext context, string tempTable, Field[] keys) {
            var values = _cf.AdoProvider == AdoProvider.Access ? string.Join(",", keys.Select(k => "?")) : string.Join(",", keys.Select(k => "@" + k.FieldName()));
            var sql = $"INSERT INTO {_cf.Enclose(tempTable)} VALUES ({values});";
            context.Debug(() => sql);
            return sql;
        }

        public IEnumerable<IRow> Read(IEnumerable<IRow> input) {
            var results = new List<IRow>();
            using (var cn = _cf.GetConnection()) {
                cn.Open();
                _context.Debug(() => "begin transaction");
                var trans = cn.BeginTransaction();

                try {
                    var createSql = SqlCreateKeysTable(_tempTable);
                    cn.Execute(createSql, null, trans);

                    var keys = input.Select(r => r.ToExpandoObject(_keys));
                    var insertSql = SqlInsertTemplate(_context, _tempTable, _keys);
                    cn.Execute(insertSql, keys, trans, 0, System.Data.CommandType.Text);

                    using (var reader = cn.ExecuteReader(SqlQuery(), null, trans, 0, System.Data.CommandType.Text)) {
                        while (reader.Read()) {
                            var row = _rowCreator.Create(reader, _fields);
                            results.Add(row);
                        }
                    }

                    var sqlDrop = SqlDrop(_tempTable);
                    cn.Execute(sqlDrop, null, trans);

                    _context.Debug(() => "commit transaction");
                    trans.Commit();

                } catch (Exception ex) {
                    _context.Error(ex.Message);
                    _context.Warn("rollback transaction");
                    trans.Rollback();
                }
            }
            return results;
        }

    }
}
