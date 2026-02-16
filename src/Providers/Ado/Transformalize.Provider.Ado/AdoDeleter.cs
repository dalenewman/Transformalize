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
using System.Linq;
using Dapper;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Providers.Ado {
    public class AdoDeleter : IDelete {
        private readonly OutputContext _output;
        private readonly IConnectionFactory _cf;
        private readonly Field[] _fields;

        public AdoDeleter(OutputContext output, IConnectionFactory cf) {
            _output = output;
            _cf = cf;
            _fields = output.Entity.GetPrimaryKey().ToArray();
        }

        public void Delete(IEnumerable<IRow> rows) {

            var criteria = string.Join(" AND ", _output.Entity.GetPrimaryKey().Select(f => f.FieldName()).Select(n => _cf.Enclose(n) + " = @" + n));
            var sql = $"UPDATE {_cf.Enclose(_output.Entity.OutputTableName(_output.Process.Name))} SET {_output.Entity.TflDeleted().FieldName()} = CAST(1 AS BIT), {_output.Entity.TflBatchId().FieldName()} = {_output.Entity.BatchId} WHERE {criteria}";
            _output.Debug(()=>sql);

            var count = (uint)0;
            using (var cn = _cf.GetConnection()) {
                cn.Open();
                foreach (var batch in rows.Partition(_output.Entity.DeleteSize)) {
                    var trans = cn.BeginTransaction();
                    var batchCount = Convert.ToUInt32(cn.Execute(
                        sql,
                        batch.Select(r => r.ToExpandoObject(_fields)),
                        trans,
                        0,
                        CommandType.Text
                        ));
                    trans.Commit();
                    count += batchCount;
                }
                _output.Entity.Deletes += count;

                if (_output.Entity.Deletes > 0) {
                    _output.Info("{0} deletes from {1}", _output.Entity.Deletes, _output.Connection.Name);
                }
            }

        }
    }
}