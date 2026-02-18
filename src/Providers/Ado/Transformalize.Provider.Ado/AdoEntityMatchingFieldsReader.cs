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
using System.Linq;
using Dapper;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Providers.Ado.Ext;

namespace Transformalize.Providers.Ado {

    public class AdoEntityMatchingFieldsReader {

        readonly InputContext _input;
        private readonly IConnectionFactory _cf;
        readonly Field[] _keys;
        readonly string _insert;
        readonly string _query;
        readonly AdoRowCreator _rowCreator;
        readonly string _create;
        readonly string _drop;

        public AdoEntityMatchingFieldsReader(InputContext input, IConnectionFactory cf, IRowFactory rowFactory) {

            var tempTable = input.Entity.GetExcelName();
            _keys = input.Entity.GetPrimaryKey();
            _input = input;
            _cf = cf;
            _create = SqlCreateKeysTable(input, cf, tempTable);
            _insert = SqlInsertTemplate(input, tempTable);
            _query = SqlQuery(input, cf, tempTable);
            _drop = SqlDrop(input, tempTable);
            _rowCreator = new AdoRowCreator(input, rowFactory);
        }

        static string SqlDrop(IConnectionContext context, string tempTable) {
            var sql = $"DROP TABLE #{tempTable};";
            context.Debug(() => sql);
            return sql;
        }

        static string SqlCreateKeysTable(IConnectionContext context, IConnectionFactory cf, string tempTable) {
            var columnsAndDefinitions = string.Join(",", context.Entity.GetPrimaryKey().Select(f => cf.Enclose(f.FieldName()) + " " + cf.SqlDataType(f) + " NOT NULL"));
            var sql = $@"CREATE TABLE #{tempTable}({columnsAndDefinitions})";
            context.Debug(() => sql);
            return sql;
        }

        static string SqlQuery(InputContext context, IConnectionFactory cf, string tempTable) {
            var keys = context.Entity.GetPrimaryKey();
            var names = string.Join(",", context.InputFields.Select(f => "i." + cf.Enclose(f.Name)));
            var table = context.SqlSchemaPrefix(cf) + cf.Enclose(context.Entity.Name);
            var joins = string.Join(" AND ", keys.Select(f => "i." + cf.Enclose(f.Name) + " = k." + cf.Enclose(f.FieldName())));
            var sql = $"SELECT {names} FROM #{tempTable} k INNER JOIN {table} i ON ({joins})";
            context.Debug(() => sql);
            return sql;
        }

        string SqlInsertTemplate(IConnectionContext context, string tempTable) {
            var sql = $"INSERT #{tempTable} VALUES ({string.Join(",", _keys.Select(k => "@" + k.FieldName()))});";
            context.Debug(() => sql);
            return sql;
        }

        public IEnumerable<IRow> Read(IEnumerable<IRow> input) {

            using (var cn = _cf.GetConnection()) {
                cn.Open();
                var trans = cn.BeginTransaction();

                cn.Execute(_create, null, trans);

                var keys = input.Select(r => r.ToExpandoObject(_keys));
                cn.Execute(_insert, keys, trans, 0, System.Data.CommandType.Text);

                using (var reader = cn.ExecuteReader(_query, null, trans, 0, System.Data.CommandType.Text)) {
                    while (reader.Read()) {
                        yield return _rowCreator.Create(reader, _input.InputFields);
                    }
                }

                cn.Execute(_drop, null, trans);
                trans.Commit();
            }
        }

    }
}
